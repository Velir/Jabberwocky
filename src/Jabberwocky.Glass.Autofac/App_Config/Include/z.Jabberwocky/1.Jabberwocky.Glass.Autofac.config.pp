<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">

  <sitecore>
    
    <!-- Factory Configuration -->
    <factories>
      <factory id="Autofac" type="Jabberwocky.Glass.Autofac.DependencyInjection.Factories.AutofacSitecoreFactory, Jabberwocky.Glass.Autofac" />
    </factories>

		<!-- Dependency Injection Service Provider -->
		<serviceProviderBuilder>
			<patch:attribute name="type" value="Jabberwocky.Glass.Autofac.DependencyInjection.AutofacServiceProviderBuilder, Jabberwocky.Glass.Autofac" />
		</serviceProviderBuilder>

		<!-- Remove Microsoft Dependency Injection Autowire Configurator (we'll use Autofac version instead) -->
		<services>
			<configurator type="Jabberwocky.DependencyInjection.Sc.Configuration.AutowireServiceConfigurator, Jabberwocky.DependencyInjection.Sc">
				<patch:delete />
			</configurator>
		</services>
		
		<!-- Autofac Dependency Registration Pipeline -->
		<pipelines>
			<initialize>
				<processor patch:before="*[1]" type="Jabberwocky.Glass.Autofac.Pipelines.Initialize.InitializeAutofacProvider, Jabberwocky.Glass.Autofac" />
			</initialize>
				
			<registerAutofacDependencies>
				<processor type="Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.ConfigureScannedAssemblies, Jabberwocky.Glass.Autofac">
					<ConfiguredAssemblies hint="list:AddAssembly">
						<!-- Add your Website DLL & Libraries here -->
						<solution>$rootnamespace$*</solution>
						<GlassMapper>Glass.Mapper</GlassMapper>
						<GlassMapperSc>Glass.Mapper.Sc</GlassMapperSc>
						<GlassMapperMvc>Glass.Mapper.Sc.Mvc</GlassMapperMvc>
					</ConfiguredAssemblies>
				</processor>
				<processor type="Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.AutowireDependencies, Jabberwocky.Glass.Autofac" />
				<processor type="Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.RegisterGlassServices, Jabberwocky.Glass.Autofac" />
				<processor type="Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.RegisterCacheServices, Jabberwocky.Glass.Autofac" />
				<processor type="Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.RegisterProcessors, Jabberwocky.Glass.Autofac">
					<IncludeScanAssemblies desc="Include Scanned Assemblies">true</IncludeScanAssemblies>
					<ConfiguredAssemblies hint="list:AddAssembly">
						<!-- Add your Website DLL & Libraries here and optionally set IncludeScanAssemblies to false to ignore everything else -->
					</ConfiguredAssemblies>
				</processor>
				<processor type="Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.RegisterGlassFactory, Jabberwocky.Glass.Autofac" />
				<processor type="Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.BuildContainer, Jabberwocky.Glass.Autofac" />
			</registerAutofacDependencies>
		</pipelines>
	
  </sitecore>

</configuration>