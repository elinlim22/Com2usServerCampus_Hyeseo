// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using HiveServer.Data.UserData;
using HiveServer.Services;
// using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using StackExchange.Redis;

// builder에 대한 정보를 설정하는 클래스
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // 이 메소드는 서비스를 컨테이너에 추가합니다.
	// 컨테이너에 추가한다는 것은 의존성 주입을 통해 객체를 생성하고 사용할 수 있게 하는 것이다.
	// 이를 통해 객체 간의 결합도를 낮추고 유연한 코드를 작성할 수 있다.
	// Configuration은 appsettings.json 파일의 설정을 가져온다.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

		services.AddScoped<TokenService>(); // TokenService를 서비스에 추가

        // Entity Framework Core와 MySQL 연결을 구성
        services.AddDbContext<UserData>(options =>
            options.UseMySql(Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("DefaultConnection"))));

		services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration.GetConnectionString("RedisConnection")?? "localhost:6379"));
    }

    // 이 메소드는 HTTP 요청 파이프라인을 구성합니다.
	// Program.cs와 다른점: Program.cs는 서버를 생성하고 실행하는 역할을 하지만, Startup.cs는 서버의 동작을 구성하는 역할을 한다.
	// 즉, 요청이 들어왔을 때 어떻게 처리할지에 대한 설정을 한다.

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage(); // 개발자 예외 페이지를 사용
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts(); // HSTS를 사용: HTTPS를 사용하도록 강제함
        }

        app.UseHttpsRedirection(); // HTTPS 리다이렉션을 사용
        app.UseStaticFiles(); // 정적 파일을 사용

        app.UseRouting(); // 라우팅을 사용

        app.UseAuthorization(); // 인증을 사용

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        }); // 컨트롤러를 매핑해서 적용
    }
}
