using SematecCC.Infra;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddInfrastructureLayer(builder.Configuration);


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

//app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


// If you have API controllers that don't use the default routing, you might need this:
app.MapControllers();//ترتیب مهمه . enables attribute routing. But ruins conventional routing
//XOR:
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers(); //  این خط ضروری است برای attribute routing
//});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=IndexNew}/{id?}");


app.Run();
