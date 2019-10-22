using System;
using System.Threading.Tasks;
using FreemiumMedia.Nop.Plugin.Misc.Meetup;
using Meetup.NetStandard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FreemiumMedia.Plugins.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            try
            {
                MeetupSettings settings = new MeetupSettings() { MeetupAuthToken = "5edd2d1c7674102f7066211113e9", MeetupGroupName = "Troy-Entrepreneurship-Meetup-Group" };


                var client = MeetupClient.WithApiToken(settings.MeetupAuthToken);
                var results = await client.Events.For(settings.MeetupGroupName);

                Assert.IsNotNull(results);
            }
            catch (Exception ex) {
                var temp = ex;
            }
        }
    }
}
