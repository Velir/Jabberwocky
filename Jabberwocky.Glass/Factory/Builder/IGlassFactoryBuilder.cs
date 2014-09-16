using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Factory.Implementation;

namespace Jabberwocky.Glass.Factory.Builder
{
	public interface IGlassFactoryBuilder
	{
		IGlassInterfaceFactory BuildFactory(IImplementationFactory implFactory, Func<ISitecoreService> serviceFactory);
	}
}
