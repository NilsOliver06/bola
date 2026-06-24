using Microsoft.EntityFrameworkCore;
using LOGIN.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURACIÓN DE LA BASE DE DATOS (PostgreSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. CONFIGURACIÓN DE CONTROLADORES Y VISTAS (MVC)
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// 3. CONFIGURACIÓN DE COOKIES Y SESIONES
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

var app = builder.Build();

// 4. MIGRACIÓN AUTOMÁTICA AL ARRANCAR EN PRODUCCIÓN
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.Migrate();
        Console.WriteLine("✅ Migraciones aplicadas correctamente y tablas verificadas.");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Error al aplicar migraciones automáticas");
        Console.WriteLine($"❌ Error: {ex.Message}");
    }
}

// 5. CONFIGURACIÓN DEL ENTORNO (Manejo de errores)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // ✅ Redirección HTTPS activa SOLO en desarrollo local.
    // Railway maneja el cifrado SSL externamente; forzarlo adentro puede romper la red interna.
    app.UseHttpsRedirection();
}

// 6. PIPELINE DE MIDDLEWARES (El orden aquí es estricto e inalterable)
app.UseStaticFiles();
app.UseRouting();

app.UseSession();       // 1° Habilitar sesiones antes de autorizar
app.UseAuthorization(); // 2° Validar permisos

// 7. ENRUTAMIENTO DE LA APLICACIÓN
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllers();

app.Run();