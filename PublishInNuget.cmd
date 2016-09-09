nuget setApiKey bb179987-155b-4629-b1cd-e7b6c5c48a6d -Source https://www.nuget.org/api/v2/package

cd C:\SOMA\Deeds\OpenSourceProjs\MultiTenancyFramework\MultiTenancyFramework.Core
nuget pack MultiTenancyFramework.Core.csproj -Sym -Prop Configuration=ReleasePackage
nuget push MultiTenancyFramework.Core.1.0.0.5.nupkg -Source https://www.nuget.org/api/v2/package

cd C:\SOMA\Deeds\OpenSourceProjs\MultiTenancyFramework\MultiTenancyFramework.NHibernate
nuget pack MultiTenancyFramework.NHibernate.csproj -Sym -Prop Configuration=ReleasePackage
nuget push MultiTenancyFramework.NHibernate.1.0.0.5.nupkg -Source https://www.nuget.org/api/v2/package

cd C:\SOMA\Deeds\OpenSourceProjs\MultiTenancyFramework\IoCContainers\MultiTenancyFramework.SimpleInjector
nuget pack MultiTenancyFramework.SimpleInjector.csproj -Sym -Prop Configuration=ReleasePackage
nuget push MultiTenancyFramework.SimpleInjector.1.0.0.5.nupkg -Source https://www.nuget.org/api/v2/package

cd C:\SOMA\Deeds\OpenSourceProjs\MultiTenancyFramework\MVC5\MultiTenancyFramework.Mvc
nuget pack MultiTenancyFramework.Mvc.csproj -Sym -Prop Configuration=ReleasePackage
nuget push MultiTenancyFramework.Mvc.1.0.0.5.nupkg -Source https://www.nuget.org/api/v2/package

cd C:\SOMA\Deeds\OpenSourceProjs\MultiTenancyFramework\MVC5\MultiTenancyFramework.Mvc.NHibernate
nuget pack MultiTenancyFramework.Mvc.NHibernate.csproj -Sym -Prop Configuration=ReleasePackage
nuget push MultiTenancyFramework.Mvc.NHibernate.1.0.0.5.nupkg -Source https://www.nuget.org/api/v2/package

