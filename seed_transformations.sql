SET NOCOUNT ON;

INSERT INTO Transformations (MemberName,MemberTitle,Description,WeightLost,DurationMonths,StartBmi,EndBmi,Goal,BeforeImageUrl,AfterImageUrl,IsActive,[Order])
VALUES
(N'علی رضایی',N'مهندس نرم‌افزار',
N'۲ سال پشت میز نشستن و نداشتن تحرک، ۱۵ کیلو اضافه وزن آورده بود. با برنامه ترکیبی تمرین و رژیم تهیلا، در ۴ ماه به وزن ایده‌آل رسیدم.',
15,4,31.2,24.8,N'کاهش وزن',
N'https://images.unsplash.com/photo-1583454110551-21f2fa2afe61?w=400&q=80',
N'https://images.unsplash.com/photo-1571731956672-f2b94d7dd0cb?w=400&q=80',
1,1),

(N'سارا محمدی',N'معلم',
N'بعد از زایمان، کاهش وزن خیلی سخت بود. برنامه تغذیه شخصی و تمرین‌های HIIT تهیلا واقعاً معجزه کرد. ۱۲ کیلو در ۵ ماه!',
12,5,28.5,22.1,N'کاهش وزن',
N'https://images.unsplash.com/photo-1490645935967-10de6ba17061?w=400&q=80',
N'https://images.unsplash.com/photo-1518310383802-640c2de311b2?w=400&q=80',
1,2),

(N'محمد کریمی',N'دانشجوی پزشکی',
N'می‌خواستم حجم بگیرم اما نمی‌دانستم چطور بخورم. رژیم پروتئین بالا و برنامه Push/Pull/Legs تهیلا، در ۶ ماه ۸ کیلو عضله اضافه کردم.',
0,6,19.2,23.5,N'افزایش حجم',
N'https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=400&q=80',
N'https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=400&q=80',
1,3),

(N'نگار احمدی',N'خانه‌دار',
N'همیشه ورزش می‌کردم اما فرم نداشتم. برنامه تمرینی هدفمند و تغذیه متعادل تهیلا، بدنم را کاملاً متحول کرد. بدون از دست دادن زیاد وزن!',
5,3,23.8,22.2,N'فرم‌دهی',
N'https://images.unsplash.com/photo-1506629082955-511b1aa562c8?w=400&q=80',
N'https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=400&q=80',
1,4),

(N'رضا قاسمی',N'راننده',
N'۴۵ سالمه و فکر می‌کردم دیگه دیر شده. اما با برنامه مناسب سن، در ۶ ماه ۱۸ کیلو کم کردم و قند خونم نرمال شد!',
18,6,34.5,26.1,N'کاهش وزن',
N'https://images.unsplash.com/photo-1538805060514-97d9cc17730c?w=400&q=80',
N'https://images.unsplash.com/photo-1581009146145-b5ef050c2e1e?w=400&q=80',
1,5),

(N'فاطمه نوری',N'کارشناس حسابداری',
N'بعد از ۳ سال بی‌تحرکی، حتی ۵ دقیقه راه رفتن نفسم می‌گرفت. الان ۳ ماهه که HIIT تمرین می‌کنم و احساس می‌کنم ۱۰ سال جوون‌تر شدم.',
8,3,27.3,23.8,N'سلامت عمومی',
N'https://images.unsplash.com/photo-1490645935967-10de6ba17061?w=400&q=80',
N'https://images.unsplash.com/photo-1518310383802-640c2de311b2?w=400&q=80',
1,6);

PRINT 'Transformations seeded OK';
