-- Delete partial data and re-insert cleanly
DELETE FROM ClassSessions;

DECLARE @c1 INT=(SELECT Id FROM OfflineClasses WHERE [Order]=1);
DECLARE @c2 INT=(SELECT Id FROM OfflineClasses WHERE [Order]=2);
DECLARE @c3 INT=(SELECT Id FROM OfflineClasses WHERE [Order]=3);
DECLARE @c4 INT=(SELECT Id FROM OfflineClasses WHERE [Order]=4);

-- Course 1: Bodybuilding
INSERT INTO ClassSessions (OfflineClassId,Title,Description,VideoUrl,ThumbnailUrl,SessionNumber,IsFree,DurationMinutes,[Order]) VALUES
(@c1,N'جلسه 1 - آشنایی با تجهیزات و گرم کردن',N'نحوه صحیح گرم کردن بدن قبل از تمرین و آشنایی کامل با تجهیزات باشگاه.','https://www.youtube.com/embed/IODxDxX7oi4','https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=300&q=70',1,1,25,1),
(@c1,N'جلسه 2 - تمرین سینه و سه سر',N'حرکات اصلی برای تقویت عضلات سینه و پشت بازو با فرم صحیح.','https://www.youtube.com/embed/vc1E5CfRfos','https://images.unsplash.com/photo-1571019614242-c5c5dee9f50b?w=300&q=70',2,0,35,2),
(@c1,N'جلسه 3 - تمرین پشت و دو سر',N'حرکات Pull برای تقویت عضلات کمر، پشت و جلو بازو.','https://www.youtube.com/embed/2tM1LFFxeKg','https://images.unsplash.com/photo-1540497077202-7c8a3999166f?w=300&q=70',3,0,38,3),
(@c1,N'جلسه 4 - تمرین پا و کشش نهایی',N'تمرینات قدرتی پا به همراه برنامه کامل کشش و سرد کردن بدن.','https://www.youtube.com/embed/QRd_7y4tRlA','https://images.unsplash.com/photo-1517838277536-f5f99be501cd?w=300&q=70',4,0,40,4);

-- Course 2: Yoga
INSERT INTO ClassSessions (OfflineClassId,Title,Description,VideoUrl,ThumbnailUrl,SessionNumber,IsFree,DurationMinutes,[Order]) VALUES
(@c2,N'جلسه 1 - یوگای صبحگاهی برای مبتدیان',N'شروع روز با یوگای آرام. حرکات پایه و تنفس آگاهانه برای مبتدیان.','https://www.youtube.com/embed/v7AYKMP6rOE','https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=300&q=70',1,1,30,1),
(@c2,N'جلسه 2 - یوگای قدرتی و انعطاف',N'ترکیب حرکات قدرتی و کششی برای افزایش انعطاف و استحکام بدن.','https://www.youtube.com/embed/149Iac5fmoE','https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=300&q=70',2,0,35,2),
(@c2,N'جلسه 3 - یوگای تعادل و تمرکز',N'حرکات تعادلی پیشرفته تر همراه با مدیتیشن برای آرامش ذهن.','https://www.youtube.com/embed/F9KGZjJkMSk','https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=300&q=70',3,0,40,3),
(@c2,N'جلسه 4 - یوگای شبانه و ریلکسیشن',N'جلسه آرام بخش قبل از خواب برای کاهش استرس و بهبود کیفیت خواب.','https://www.youtube.com/embed/BiWDsfZ3zbo','https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=300&q=70',4,0,25,4);

-- Course 3: HIIT
INSERT INTO ClassSessions (OfflineClassId,Title,Description,VideoUrl,ThumbnailUrl,SessionNumber,IsFree,DurationMinutes,[Order]) VALUES
(@c3,N'جلسه 1 - HIIT مبتدی 20 دقیقه',N'آشنایی با مفهوم تمرین اینتروال. 10 حرکت پایه با استراحت کافی.','https://www.youtube.com/embed/ml6cT4AZdqI','https://images.unsplash.com/photo-1598971457999-ca4ef48a9a71?w=300&q=70',1,1,22,1),
(@c3,N'جلسه 2 - HIIT متوسط 30 دقیقه',N'فشار بیشتر، استراحت کمتر. ترکیب قدرت و استقامت.','https://www.youtube.com/embed/UItWltVZZmE','https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=300&q=70',2,0,32,2),
(@c3,N'جلسه 3 - HIIT پیشرفته Tabata',N'پروتکل تاباتا: 20 ثانیه تمرین، 10 ثانیه استراحت. سخت ترین جلسه.','https://www.youtube.com/embed/oQ1PnO9A4jI','https://images.unsplash.com/photo-1598971457999-ca4ef48a9a71?w=300&q=70',3,0,28,3),
(@c3,N'جلسه 4 - HIIT با دمبل',N'همان شدت HIIT با وزنه دستی برای عضله سازی همزمان.','https://www.youtube.com/embed/cbKkB3POqaY','https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=300&q=70',4,0,35,4);

-- Course 4: Pilates
INSERT INTO ClassSessions (OfflineClassId,Title,Description,VideoUrl,ThumbnailUrl,SessionNumber,IsFree,DurationMinutes,[Order]) VALUES
(@c4,N'جلسه 1 - مبانی پیلاتس و فعال سازی هسته',N'اصول پایه پیلاتس، تنفس صحیح و فعال سازی عضلات عمقی شکم.','https://www.youtube.com/embed/g_tea8ZNk5A','https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=300&q=70',1,1,30,1),
(@c4,N'جلسه 2 - پیلاتس کمر و باسن',N'حرکات تخصصی برای تقویت عضلات کمری و رفع کمردرد مزمن.','https://www.youtube.com/embed/K64b0zoqPFU','https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=300&q=70',2,0,35,2),
(@c4,N'جلسه 3 - پیلاتس کامل بدن 40 دقیقه',N'تمرین جامع تمام بدن. ترکیب حرکات کلاسیک پیلاتس.','https://www.youtube.com/embed/VaoV1PrYft4','https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=300&q=70',3,0,40,3);

PRINT 'All 15 sessions inserted successfully';
