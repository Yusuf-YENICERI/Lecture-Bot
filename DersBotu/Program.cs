using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DersBotu
{
    class Program
    {
        public static Dictionary<long, int> kosul = new Dictionary<long, int>();
        public static bool addPhoto = false;
        public static Ana dersDb = new Ana();
        public static Kategoriler kategoriDb = new Kategoriler();
        public static Kategoriler kategoriDb2 = new Kategoriler();
        public static readonly TelegramBotClient botClient = new TelegramBotClient("");
        private static Dictionary<long, string> KategoriName = new Dictionary<long, string>();
        static void Main(string[] args)
        {
            try
            {
                #region Database nesneleri
                //Tüm db nesnelerini nulla ayarla
                dersDb = null;
                kategoriDb = null;
                kategoriDb2 = null;
                #endregion
                #region Bot için Gerekli Bilgiler
                //Bot bilgilerini çek
                var me = botClient.GetMeAsync().Result;
                //Konsol yazısını değiştir
                Console.Title = me.Username;
                #endregion
                #region Event Uyandırma
                //Gerekli eventleri ayarla

                botClient.OnMessage += OyunOyna;
                botClient.OnMessage += BotOnMessageReceived;
                botClient.OnMessage += KosulKontrolü;
                botClient.OnMessageEdited += BotOnMessageReceived;
                #endregion
                #region Mesajları Alma
                //Mesajları almaya başla
                botClient.StartReceiving(Array.Empty<UpdateType>());
                Console.WriteLine($"@{me.Username} için dinleme başladı");

                //Program açık kalsın
                while (true) {}

                //Mesajları almayı sonlandır
                botClient.StopReceiving();
                #endregion
            }
            catch ( Exception e)
            {
                #region Hata Bilgilendirmesi
                HataYazdir.Function(e);
                #endregion
                Console.Read();
            }
        }
        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            string concat = string.Empty;
            try
            {
                #region Kullanıcı Ekleme ve Blok Kontrolü
                //Kullanıcıyı Id sinden bul
                using (botDatabase db = new botDatabase())
                {
                    var kullanici = db.Kullanicilars.Where(a => a.UserId == message.From.Id).FirstOrDefault();

                    //kullanici nullsa koşul sun
                    if (kullanici == null)
                    {
                        #region Koşul Kısmı
                        if (!kosul.ContainsKey(message.Chat.Id))
                        {
                            kosul[message.Chat.Id] = 0;
                            ReplyKeyboardMarkup ReplyKeyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
                                       {
                                        new KeyboardButton[]
                                        {
                                            "Evet","Hayır"
                                        }
                                       })
                            { ResizeKeyboard = true };
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Bu bot ders notları sunar.\nKonuşmaya başlamadan önce:\n\n-Kullanıcının, *Telegram Bot API* de *User Type*(_Kullanıcı Tipi_)'ı için belirtilen\n --*first_name*_(İlk isim)_,\n --*last_name*_(Son isim)_,\n --*username*_(Kullanıcı Adi)_,\n --*id*_(Kullanıcı Id)_ değerleri,\n\n-Bota gönderilen mesajlar,\n\nDaha iyi hizmet sunmak(gerekli olduğunda bildirim yollamak, komutları basitleştirmek) için saklanır.\n\nDevam etmek istiyor musunuz?", ParseMode.Markdown, replyMarkup: ReplyKeyboard);
                            
                        }
                        else if (kosul[message.Chat.Id] == 1)
                        {
                            kosul.Remove(message.Chat.Id);
                        }
                        else if (message.Text != "Evet" && message.Text != "Hayır")
                        {
                            ReplyKeyboardMarkup ReplyKeyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
                                       {
                                        new KeyboardButton[]
                                        {
                                            "Evet","Hayır"
                                        }
                                       })
                            { ResizeKeyboard = true };
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Bu bot ders notları sunar.\nKonuşmaya başlamadan önce:\n\n-Kullanıcının, *Telegram Bot API* de *User Type*(_Kullanıcı Tipi_)'ı için belirtilen\n --*first_name*_(İlk isim)_,\n --*last_name*_(Son isim)_,\n --*username*_(Kullanıcı Adi)_,\n --*id*_(Kullanıcı Id)_ değerleri,\n\n-Bota gönderilen mesajlar,\n\nDaha iyi hizmet sunmak(gerekli olduğunda bildirim yollamak, komutları basitleştirmek) için saklanır.\n\nDevam etmek istiyor musunuz?", ParseMode.Markdown, replyMarkup: ReplyKeyboard);
                            
                        }
                        #endregion
                    }
                    else
                    {

                        //Eğer kullanıcı bloklandıysa içine girme (bloklandı = 0)
                        if (kullanici.Blocked == 1)
                        {
                            #endregion

                            if (message.Type == MessageType.VideoMessage)
                            {
                                Console.WriteLine(message.Video.FileId);
                            }

                            #region Photo Ekle
                            if (message != null && message.Type == MessageType.PhotoMessage)
                            {

                                db.KullaniciNotlars.Add(new KullaniciNotlar { KullaniciId = kullanici.KullanicilarId, NotStringi = message.Photo[message.Photo.Length-1].FileId });
                                db.SaveChanges();
                                

                                if (addPhoto)
                                {
                                    //Eğer kategori eklenmediyse fotoğraf ekleme ve uyarı yazdır
                                    if (kategoriDb2 != null)
                                    {
                                        db.Sayfalars.Add(new Sayfalar { KategoriId = kategoriDb2.KategoriId, SayfaStringi = message.Photo[3].FileId, Caption = message.Caption });
                                        db.SaveChanges();
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Taked!");
                                    }
                                    else
                                    {
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Eklenemedi!");
                                    }
                                }

                            }
                            #endregion

                            if (message != null && message.Type == MessageType.TextMessage)
                            {
                                 
                                #region AddPhoto İşlemleri
                                if (addPhoto)
                                {
                                    if (message.Text[0] != '+')
                                    {
                                        kategoriDb2 = db.Kategorilers.Where(a => a.KategoriIsmi.ToUpper() == message.Text.ToUpper()).FirstOrDefault();
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Kategori seçildi!");
                                    }
                                    else
                                    {
                                        string compare = message.Text;
                                        compare = compare.Remove(0, 1);

                                        switch (compare[0])
                                        {
                                            case '#':
                                                compare = compare.Remove(0, 1);
                                                dersDb = db.Anas.Where(a => a.Dersler.ToUpper() == compare.ToUpper()).FirstOrDefault();
                                                if (dersDb == null)
                                                {
                                                    dersDb = db.Anas.Add(new Ana { Dersler = compare });
                                                    db.SaveChanges();
                                                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ders Eklendi!");
                                                }
                                                break;
                                            case '@':
                                                compare = compare.Remove(0, 1);
                                                kategoriDb = db.Kategorilers.Where(a => a.KategoriIsmi.ToUpper() == compare.ToUpper()).FirstOrDefault();
                                                if (dersDb != null && kategoriDb == null)
                                                {
                                                    kategoriDb = db.Kategorilers.Add(new Kategoriler { KategoriIsmi = compare, DersId = dersDb.DersId });
                                                    db.SaveChanges();
                                                    await botClient.SendTextMessageAsync(message.Chat.Id, "Kategori Eklendi!");
                                                }
                                                else
                                                {
                                                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ders Gir!");
                                                }
                                                break;
                                        }
                                    }
                                }
                                #endregion

                                #region AddPhoto Check
                                if (message.Text == "addPhoto")
                                {
                                    if (message.From.FirstName == kullanici.İsim && message.From.LastName == kullanici.Soyisim && message.From.Id == kullanici.UserId && message.From.Username == kullanici.KullaniciAdi && kullanici.Admin == 1)
                                    {
                                        addPhoto = true;
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Ekleme işlemlerine hazır!");
                                    }
                                    else
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Yanlış sularda yüzüyorsun!");
                                }

                                if (message.Text == "closePhoto" && message.From.FirstName == "J" && message.From.LastName == "H")
                                {
                                    addPhoto = false;
                                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ekleme işlemi bitti!");
                                }
                                #endregion

                                #region /komutlar
                                switch (message.Text)
                                {
                                    #region /start
                                    case "/start":
                                        concat = string.Empty;
                                        concat += "_Merhaba, Ben Not Canavarı! Ben not vermek için burdayım. _*Derse katılamadıysan ve notlara ihtiyacın varsa en doğru yerdesin.*_ Notları sana vereceğim, ancak unutma ki en iyi not derste alınır. Hadi başlayalım _*" + message.From.FirstName + ' ' + message.From.LastName + ".*_ \n\nDers seçerken ders isminin başına bir boşlukla \"/ders\" koy. Örnek: _*/ders Lineer Cebir*_.\n\nKonu seçerken ise konu isminin başına \"/konu\" koy. Örnek: _*/konu Düzlemde Vektörler*_. _*\n\nİşte not verebildiğim dersler :*\n\n";
                                        //lessons adındaki değişkene dersleri aktar ve teker teker ekrana yazdır
                                        var lessons = db.Anas.Select(x => x.Dersler).ToList();
                                        foreach (var item in lessons)
                                        {
                                            concat += "/ders " + item + "\n";
                                        }
                                        await botClient.SendTextMessageAsync(message.Chat.Id, concat, ParseMode.Markdown);
                                        await botClient.SendVideoAsync(message.Chat.Id, new FileToSend("BAADBAADfgUAAjNrGFBMdZB_Dn7xBQI"));
                                        break;
                                    #endregion

                                    #region /oyun_oyna
                                    case "/oyun_oyna":
                                        //Ekrana klavyeyi yazdır
                                        ReplyKeyboardMarkup ReplyKeyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
                                        {
                                        new KeyboardButton[]
                                        {
                                            "0","1","2","3",
                                        },
                                        new KeyboardButton[]
                                        {
                                            "4","5","6","7","8"
                                        },
                                        new KeyboardButton[]
                                        {
                                           "9", "10","11","12",
                                        },
                                        new KeyboardButton[]
                                        {
                                           "13", "14","15","16",
                                        },
                                        new KeyboardButton[]
                                        {
                                           "17", "18","19","20"
                                        }
                                        });
                                        concat = string.Empty;
                                        concat += "Ders çalışmaktan canın mı sıkıldı? O zaman benle bir *oyun oyna!*\n";
                                        concat += "Eeee, Hadi Başlayalım o zaman! Oyundaki amaç 0-20 arasında benim tuttuğum sayıya 3 farkla denk gelmek. Eğer gelemezsen *KAYBEDİYORSUN*. Fakat 3 farkla denk gelirsen sıkıntı yok, takılmaya devam edebiliriz. Örnek: Bana 5 sayısını gönderdin. Bense 15 sayısını tutmuştum, o zaman kaybedersin. Eğer 3 sayısını gönderdinse ve ben 6 tuttuysam *KAZANIYORSUN*.\n";
                                        await botClient.SendTextMessageAsync(message.Chat.Id, concat, ParseMode.Markdown, replyMarkup: ReplyKeyboard);
                                        //Kullaniciyi oyun moduna sok
                                        db.Kullanicilars.Where(a => a.UserId == message.From.Id).FirstOrDefault().Oyun.Oynuyor = true;
                                        db.SaveChanges();
                                        break;
                                    #endregion

                                    #region /oyunu_bitir
                                    case "/oyunu_bitir":
                                        //Kullanici oynamayi bitirdi
                                        db.Kullanicilars.Where(a => a.UserId == message.From.Id).FirstOrDefault().Oyun.Oynuyor = false;
                                        db.SaveChanges();
                                        //Klavyeyi kaldır
                                        ReplyKeyboardRemove remove = new ReplyKeyboardRemove() { RemoveKeyboard = true };
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Vee oyun biter... Derse devam!", replyMarkup: remove);
                                        break;
                                    #endregion

                                    #region /sirala
                                    case "/sirala":
                                        int i = 1;
                                        concat = string.Empty;
                                        concat += "Bakalım neredesin!\n";
                                        //Paunları sıralayarak al
                                        var sirala = db.Oyuns.OrderByDescending(a => a.Puan).ToList();
                                        //İlk 10 u yazdır
                                        foreach (var item in sirala)
                                        {
                                            if (i < 11)
                                                concat += item.Kullanicilar.İsim + " " + item.Kullanicilar.Soyisim + " " + item.Puan + "\n";
                                            i++;
                                        }

                                        //Şuanki kullanıcının sırasını belirle
                                        int sira = sirala.IndexOf(db.Oyuns.OrderByDescending(a => a.Puan & a.Kazandı - a.Kaybetti).Where(a => a.Kullanicilar.UserId == kullanici.UserId).FirstOrDefault());

                                        concat += "Şuanda " + (sira + 1) + "." + " sıradasın.";
                                        //Mesajı gönder
                                        await botClient.SendTextMessageAsync(message.Chat.Id, concat);
                                        break;
                                    #endregion

                                    #region Ders ve Kategori Göster
                                    default:
                                        //Öncelikle alt stringi al
                                        if (message.Text.Length >= 6)
                                        {
                                            string dersKategoriKarsilastir = message.Text.Substring(0, 6);
                                            switch (dersKategoriKarsilastir)
                                            {
                                                #region /ders
                                                case "/ders ":
                                                    //Komut kısmını kaldır
                                                    string compare = message.Text;
                                                    compare = compare.Remove(0, 6);

                                                    //Veritabanında dersi bul
                                                    var ders = db.Anas.Where(a => a.Dersler.ToUpper() == compare.ToUpper()).FirstOrDefault();
                                                    if (ders != null)
                                                    {
                                                        //Kategorileri çek ve listele
                                                        var listele = ders.Kategorilers.Where(a => a.DersId == ders.DersId).Select(x => x.KategoriIsmi).ToList();
                                                        if (listele != null)
                                                        {
                                                            concat = string.Empty;
                                                            foreach (var item in listele)
                                                            {
                                                                concat += "/konu " + item + "\n";
                                                            }
                                                            await botClient.SendTextMessageAsync(message.Chat.Id, concat, ParseMode.Default);
                                                        }
                                                        else
                                                        {
                                                            await botClient.SendTextMessageAsync(message.Chat.Id, message.Text.Remove(0, 6) + " dersine ait herhangi bir konu bulunamadi. Unutma, dersi girerken büyük ya da küçük harf girmen önemli değil, ancak boşluklar... Onlara dikkat et!");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Ders bulunamadı! _Lütfen /ders ten sonra bir boşlukla ders ismini gir!_ Örnek komut: */ders Lineer Cebir*", ParseMode.Markdown);
                                                    }
                                                    break;
                                                #endregion

                                                #region /konu
                                                case "/konu ":
                                                    //Komutu kaldır
                                                    string karsila = message.Text;
                                                    karsila = karsila.Remove(0, 6);
                                                    //Konuyu bul
                                                    var kategori = db.Kategorilers.Where(a => a.KategoriIsmi.ToUpper() == karsila.ToUpper()).FirstOrDefault();
                                                    if (kategori != null)
                                                    {
                                                        //Ekrana yazdır
                                                        var sayfalar = kategori.Sayfalars.Where(a => a.KategoriId == kategori.KategoriId).ToList();
                                                        if (sayfalar.Capacity > 0)
                                                        {
                                                            int ord = 1;
                                                            foreach (var item in sayfalar)
                                                            {
                                                                if (item.Caption != null)
                                                                    await botClient.SendPhotoAsync(message.Chat.Id, new Telegram.Bot.Types.FileToSend(item.SayfaStringi), ord + ", " + item.Caption, true);
                                                                else
                                                                    await botClient.SendPhotoAsync(message.Chat.Id, new Telegram.Bot.Types.FileToSend(item.SayfaStringi), ord.ToString(), disableNotification: true);
                                                                ord++;
                                                            }
                                                        }
                                                        else
                                                            await botClient.SendTextMessageAsync(message.Chat.Id, "Herhangi bir not bulunamadı!\n\nEğer bu konuya not eklemek istiyorsan, çektiğin fotoğrafları direk gönder!");
                                                    }
                                                    else
                                                    {
                                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Konu bulunamadı! _Lütfen /konu dan sonra bir boşlukla konu ismini gir!_ Örnek komut: */konu Alt Uzayların Boyutları*", ParseMode.Markdown);
                                                    
                                                    }
                                                    break;
                                                    #endregion
                                            }
                                        }
                                        else if (message.Text.Contains("/ders"))
                                        {
                                            await botClient.SendTextMessageAsync(message.Chat.Id, "_Lütfen /ders ten sonra bir boşlukla ders ismini gir!_ Örnek komut: */ders Lineer Cebir*", ParseMode.Markdown);
                                        }
                                        else if (message.Text.Contains("/konu"))
                                        {
                                            await botClient.SendTextMessageAsync(message.Chat.Id, "_Lütfen /konu dan sonra bir boşlukla konu ismini gir!_ Örnek komut: */konu Alt Uzayların Boyutları*", ParseMode.Markdown);
                                        }
                                        break;
                                        #endregion

                                }
                                #endregion

                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.From.Id, "Bloklandın!");
                        }
                        Console.Write(message.Date+" " + kullanici.İsim + " " + kullanici.Soyisim+ ": ");
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(message.Text);
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("  " + "Mesaj Tipi: ");
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(message.Type);
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        db.Yorumlars.Add(new Yorumlar { KullaniciId = kullanici.KullanicilarId, Yorumlar1 = message.Text });
                        db.SaveChanges();
                    }
                }

            }
            catch (Exception e)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Bir hata oluştu. Son mesajı bir daha atar mısın?");
                #region Hata Bilgilendirmesi
                HataYazdir.Function(e);
                #endregion
            }
        }


        private static async void OyunOyna(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            var chatId = message.Chat.Id;
            string birlestir = string.Empty;
            using (botDatabase db = new botDatabase())
            {
                //Kullaniciyi al
                var kullanici = db.Kullanicilars.Where(a => a.UserId == message.From.Id).FirstOrDefault();

                if (kullanici != null && kullanici.Blocked == 1 && kullanici.Oyun.Oynuyor == true && message.Type == MessageType.TextMessage)
                {

                    try
                    {
                        Random rnd = new Random();
                        int userSayi;
                        int randomSayi = rnd.Next(0, 21);
                        if (int.TryParse(message.Text, out userSayi))
                        {
                            if (randomSayi + 3 >= userSayi && randomSayi - 3 <= userSayi)
                            {
                                kullanici.Oyun.Kazandı += 1;
                                kullanici.Oyun.Puan += 5;
                                db.SaveChanges();
                                birlestir = string.Empty;
                                birlestir += "İyi atış :D !\n";
                                birlestir += "Tuttuğum sayı: " + randomSayi + "\n";
                                birlestir += "Mevcut Puan: " + kullanici.Oyun.Puan;
                                await botClient.SendTextMessageAsync(message.Chat.Id, birlestir);
                            }
                            else
                            {
                                kullanici.Oyun.Kaybetti += 1;
                                kullanici.Oyun.Puan -= 3;
                                db.SaveChanges();
                                birlestir = string.Empty;
                                birlestir += "Denemeye devam!\n";
                                birlestir += "Tuttuğum sayı: " + randomSayi + "\n";
                                birlestir += "Mevcut Puan: " + kullanici.Oyun.Puan;
                                await botClient.SendTextMessageAsync(message.Chat.Id, birlestir);
                            }
                        }
                        else if (!message.Text.Contains('/'))
                        {
                            birlestir = string.Empty;
                            birlestir += "Yav o nasıl sayı?\n";
                            birlestir += "Oyunu bitirmek istiyorsan /oyunu_bitir e tıkla!";
                            await botClient.SendTextMessageAsync(message.Chat.Id, birlestir);
                        }
                    }
                    catch (Exception e)
                    {
                        #region Hata Bilgilendirmesi
                        HataYazdir.Function(e);
                        #endregion
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Hata var sanki!");
                    }
                }
            }
        }
        
        private static async void KosulKontrolü(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            string birles = string.Empty;
            if(message.Type == MessageType.TextMessage && message.Text == "Evet")
            {
                using (botDatabase db = new botDatabase())
                {

                    kosul[message.Chat.Id] = 1;
                    var al = db.Kullanicilars.Add(new Kullanicilar { KullaniciAdi = message.From.Username, İsim = message.From.FirstName, Soyisim = message.From.LastName, UserId = message.From.Id, Blocked = 1, Admin = 0 });
                    db.Oyuns.Add(new Oyun { OyunId = al.KullanicilarId, Oynuyor = false, Kazandı = 0, Kaybetti = 0, Puan = 0 });
                    db.SaveChanges();
                    ReplyKeyboardRemove remove = new ReplyKeyboardRemove() { RemoveKeyboard = true };
                    birles += "_Merhaba, Ben Not Canavarı! Ben not vermek için burdayım. _*Derse katılamadıysan ve notlara ihtiyacın varsa en doğru yerdesin.*_ Notları sana vereceğim, ancak unutma ki en iyi not derste alınır. Hadi başlayalım _*" + message.From.FirstName + ' ' + message.From.LastName + ".*_ \n\nDers seçerken ders isminin başına bir boşlukla \"/ders\" koy. Örnek: _*/ders Lineer Cebir*_.\n\nKonu seçerken ise konu isminin başına \"/konu\" koy. Örnek: _*/konu Düzlemde Vektörler*_. _*\n\nİşte not verebildiğim dersler :*\n\n";
                    //lessons adındaki değişkene dersleri aktar ve teker teker ekrana yazdır
                    var lessons = db.Anas.Select(x => x.Dersler).ToList();
                    foreach (var item in lessons)
                    {
                        birles += "/ders " + item + "\n";
                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, birles, ParseMode.Markdown,replyMarkup:remove);
                    await botClient.SendVideoAsync(message.Chat.Id, new FileToSend("BAADBAADfgUAAjNrGFBMdZB_Dn7xBQI"));
                    Console.WriteLine("\aBir kullanıcı geldi!");
                }
            }
            else if(message.Type == MessageType.TextMessage && message.Text == "Hayır"){
                ReplyKeyboardRemove remove = new ReplyKeyboardRemove() { RemoveKeyboard = true };
                await botClient.SendTextMessageAsync(message.Chat.Id, "İstediğin zaman /start komutuna tıklayarak(ya da komutu yollayarak) fikrini değiştirebilirsin.",replyMarkup:remove);
            }
            
        }
    }
}