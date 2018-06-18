namespace shopping_list

open Microsoft.AspNetCore.Authentication
open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy;
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Bot.Connector

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this._Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1) |> ignore
        
        let credentialProvider = new StaticCredentialProvider(
                                        this._Configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey).Value, 
                                        this._Configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppPasswordKey).Value
                                    )
        
        services.AddAuthentication(
                            // This can be removed after https://github.com/aspnet/IISIntegration/issues/371
                            fun (options: AuthenticationOptions) ->
                                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme |> ignore
                                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme |> ignore
                        )
                        .AddBotAuthentication(credentialProvider) |> ignore
        services.AddSingleton(typedefof<ICredentialProvider>, credentialProvider) |> ignore
           
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =

        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        else
            app.UseExceptionHandler("/Home/Error") |> ignore
            app.UseHsts() |> ignore

        app.UseHttpsRedirection() |> ignore
        app.UseStaticFiles() |> ignore
        app.UseAuthentication() |> ignore

        app.UseMvc() |> ignore
//        app.UseMvc(fun routes ->
//            routes.MapRoute(
//                name = "default",
//                template = "{controller=Home}/{action=Index}/{id?}") |> ignore
//            ) |> ignore

    member val _Configuration : IConfiguration = null with get, set
