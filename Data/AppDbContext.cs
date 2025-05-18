using AnounChatBot.Models;
using Microsoft.EntityFrameworkCore;

namespace AnounChatBot.Data
{
    public class AppDbContext:DbContext//این کلاس چیه ؟ این کلاس به EF Core میگه که چجوری با پایگاه دادمون ارتباط یرقرار کنه.واومده از DbContect ارث بری کرده که یک کلاس پایس که توشیکسری ویژگی هست تا بتونه از اون ویژگی ها استفاده کنه.
                                       //DbContext کلاس پایست ک عملیاتی مثل ذخیرهی داده ،دریافت داده و غیره ... را د رپایگاه داده مدیریت میکند.                          
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options) { }//DbContextOptions این ویژگی از دله همون DbContext یا کلاس پایه اومده.
        // EF Coreباید بداند چجوری به دیتابیس وصل شود این تنظیمات مثل اسم دیتابیس ، آدرس سرور و غیره در Optionقرار دارد.وقتی برنامه شروع میشود 
        //پس DbContextOptions<AppDbContext>options شامل اطلاعات دیتابیس است.
        //base(options) میاد این اطلاعات را به کلاس والد(DbContext)میفرسته تا EF Core بتونه ان را استفاده کنه 
        //پس، این خط فقط برای اینه که تنظیمات دیتابیس در DbContext تنظیم بشه و بقیه کدها بتونن ازش استفاده کنن.
        public DbSet<User> Users { get; set; } //جدول کاربران


        protected override void OnModelCreating(ModelBuilder modelBuilder)//ین متد به شما این امکان را می‌دهد که پیکربندی‌های خاص پایگاه داده (مانند تعریف روابط بین جداول یا اعمال محدودیت‌ها) را انجام دهید.
        {
            base.OnModelCreating(modelBuilder);//در اینجا، چون چیزی تغییر نکرده، فقط base.OnModelCreating(modelBuilder) فراخوانی شده است که به این معناست که پیکربندی پیش‌فرض انجام می‌شود.

            //شما می‌توانید در این متد کدهایی اضافه کنید که تنظیمات خاصی روی مدل‌های شما اعمال کند.
        }
    }
}
