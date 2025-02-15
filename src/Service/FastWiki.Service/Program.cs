using FastWiki.Service;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.GetSection(OpenAIOption.Name)
    .Get<OpenAIOption>();

builder.Configuration.GetSection(JwtOptions.Name)
    .Get<JwtOptions>();

builder
    .AddLoadEnvironment();

builder
    .AddFastSemanticKernel();

var app = builder.Services
    .AddAuthorization()
    .AddJwtBearerAuthentication()
    .AddMemoryCache()
    .AddEndpointsApiExplorer()
    .AddMasaIdentity()
    .AddMapster()
    .AddHttpContextAccessor()
    .AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer xxxxxxxxxxxxxxx\"",
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });

        options.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Title = "FastWiki.ServiceApp",
                Version = "v1",
                Contact = new OpenApiContact { Name = "FastWiki.ServiceApp", }
            });
        foreach (var item in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml"))
            options.IncludeXmlComments(item, true);
        options.DocInclusionPredicate((docName, action) => true);
    })
    .AddMasaDbContext<WikiDbContext>(opt => { opt.UseNpgsql(); })
    .AddDomainEventBus(dispatcherOptions =>
    {
        dispatcherOptions
            .UseEventBus()
            .UseUoW<WikiDbContext>()
            .UseRepository<WikiDbContext>();
    })
    .AddAutoInject()
    .AddServices(builder, option => option.MapHttpMethodsForUnmatched = ["Post"]);

app.UseMasaExceptionHandler();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
        .UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "FastWiki.ServiceApp"));

    #region MigrationDb

    await using var context = app.Services.CreateScope().ServiceProvider.GetService<WikiDbContext>();
    {
        await context!.Database.EnsureCreatedAsync();

        // TODO: 创建vector插件如果数据库没有则需要提供支持向量的数据库。
        await context.Database.ExecuteSqlInterpolatedAsync($"CREATE EXTENSION IF NOT EXISTS vector;");
    }

    #endregion
}

app.Run();