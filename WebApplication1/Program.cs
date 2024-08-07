using WebApplication1.Services.Hash;
using WebApplication1.Services.Kdf;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
/*̳��� ��� ��������� ����� - �� ���������� Builder �� ���� ������������� (app) 
  ��������� - ������������ ���������� � ������ �� ��������
  "���� ����� �� IHashService - ������ ��'��� ����� Md5HashService"
 
 */
builder.Services.AddSingleton<IHashService, Md5HashService>();
builder.Services.AddSingleton<IKdfService, Pbkdf1Service>();
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options => { 
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
