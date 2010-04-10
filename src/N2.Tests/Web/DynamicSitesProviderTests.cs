using System.Linq;
using N2.Tests.Web.Items;
using NUnit.Framework;
using N2.Web;
using N2.Configuration;
using N2.Persistence.NH.Finder;
using System.Collections;
using System;
using System.Collections.Generic;
using N2.Persistence.Finder;
using N2.Tests.Fakes;
using N2.Definitions;

namespace N2.Tests.Web
{
	[TestFixture]
	public class DynamicSitesProviderTests : ItemPersistenceMockingBase
	{
		ContentItem rootItem;
	    private IHost host;
	    private HostSection config;
	    private DynamicSitesProvider sitesProvider;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			rootItem = CreateTheItemTree();
            host = new Host(new Fakes.FakeWebContextWrapper(), rootItem.ID, rootItem.ID);
			mocks.ReplayAll();

		    config = new HostSection {RootID = rootItem.ID, StartPageID = rootItem.ID};
			var definitions = TestSupport.SetupDefinitions(typeof(SiteProvidingItem), typeof(PageItem));
			var finder = new FakeItemFinder(definitions, () => base.repository.database.Values.OfType<SiteProvidingItem>().Cast<ContentItem>());
			sitesProvider = new DynamicSitesProvider(persister, finder, definitions, host, config);
		}

		protected SiteProvidingItem CreateTheItemTree()
		{
			int id = 22;
			SiteProvidingItem rootItem = CreateOneItem<SiteProvidingItem>(++id, id.ToString(), null);
			SiteProvidingItem site1 = CreateOneItem<SiteProvidingItem>(++id, id.ToString(), rootItem);
			SiteProvidingItem site2 = CreateOneItem<SiteProvidingItem>(++id, id.ToString(), rootItem);
			PageItem item3 = CreateOneItem<PageItem>(++id, id.ToString(), rootItem);
			PageItem item4 = CreateOneItem<PageItem>(++id, id.ToString(), rootItem);
			PageItem item1_1 = CreateOneItem<PageItem>(++id, id.ToString(), site1);
			PageItem item1_2 = CreateOneItem<PageItem>(++id, id.ToString(), site1);
			PageItem item2_1 = CreateOneItem<PageItem>(++id, id.ToString(), site2);
			PageItem item3_1 = CreateOneItem<PageItem>(++id, id.ToString(), item3);
			PageItem item3_2 = CreateOneItem<PageItem>(++id, id.ToString(), item3);
			SiteProvidingItem site1_1 = CreateOneItem<SiteProvidingItem>(++id, id.ToString(), site1);
			SiteProvidingItem site4_1 = CreateOneItem<SiteProvidingItem>(++id, id.ToString(), item4);
			return rootItem;
		}

		[Test]
		public void CanFind_AllSites()
		{
			Assert.That(sitesProvider.GetSites().Count(), Is.EqualTo(5));
		}
	}
}
