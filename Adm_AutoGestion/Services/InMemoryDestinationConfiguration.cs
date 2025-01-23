using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAP.Middleware.Connector;
using SAPExtractorDotNET;

namespace Adm_AutoGestion
{
    public partial class InMemoryDestinationConfiguration : IDestinationConfiguration
    {
        private Dictionary<string, RfcConfigParameters> availableDestinations;
        RfcDestinationManager.ConfigurationChangeHandler changeHandler;
        public InMemoryDestinationConfiguration()
        {
            availableDestinations = new Dictionary<string, RfcConfigParameters>();
        }

        public RfcConfigParameters GetParameters(string destinationName)
        {
            RfcConfigParameters foundDestination;
            availableDestinations.TryGetValue(destinationName, out foundDestination);
            return foundDestination;
        }

        public bool ChangeEventsSupported()
        {
            return true;
        }

        public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;



        public void AddOrEditDestination(RfcConfigParameters parameters)
        {
            string name = parameters[RfcConfigParameters.Name];
            if (availableDestinations.ContainsKey(name))
            {
                var EventArgs = new RfcConfigurationEventArgs(RfcConfigParameters.EventType.CHANGED, parameters);
                ConfigurationChanged.Invoke(name, EventArgs);
                //changeHandler(name, EventArgs);


            }

            availableDestinations[name] = parameters;

        }

        public void RemoveDestination(string name)
        {
            if (availableDestinations.Remove(name))
            {

                changeHandler(name, new RfcConfigurationEventArgs(RfcConfigParameters.EventType.DELETED));


            }
        }
    }
}