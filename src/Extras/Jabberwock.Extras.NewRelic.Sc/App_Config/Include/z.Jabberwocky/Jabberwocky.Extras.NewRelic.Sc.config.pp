<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <pipelines>
      <initialize>
        <processor type="Jabberwocky.Extras.NewRelic.Sc.Pipelines.Initialize.ApplicationNameProcessor, Jabberwocky.Extras.NewRelic.Sc" />
      </initialize>
      <httpRequestBegin>
        <processor type="Jabberwocky.Extras.NewRelic.Sc.Pipelines.HttpRequestBegin.TransactionNameProcessor, Jabberwocky.Extras.NewRelic.Sc" patch:after="processor[@type='Sitecore.Pipelines.HttpRequest.ItemResolver, Sitecore.Kernel']" />
      </httpRequestBegin>
      <mvc.renderRendering>
        <processor type="Jabberwocky.Extras.NewRelic.Sc.Pipelines.RenderRendering.StartResponseTimeMetricProcessor,Jabberwocky.Extras.NewRelic.Sc" patch:before="processor[@type='Sitecore.Mvc.Pipelines.Response.RenderRendering.ExecuteRenderer, Sitecore.Mvc']" />
        <processor type="Jabberwocky.Extras.NewRelic.Sc.Pipelines.RenderRendering.EndResponseTimeMetricProcessor,Jabberwocky.Extras.NewRelic.Sc" patch:after="processor[@type='Sitecore.Mvc.Pipelines.Response.RenderRendering.ExecuteRenderer, Sitecore.Mvc']" />
      </mvc.renderRendering>
    </pipelines>
    <settings>
      <setting name="NewRelic.AppName" value="$assemblyName$" />
    </settings>
  </sitecore>
</configuration>
