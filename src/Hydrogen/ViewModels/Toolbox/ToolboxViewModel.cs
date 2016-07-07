using System.Collections.Generic;

namespace Hydrogen.ViewModels.Toolbox
{
    public class ToolSubscriptionOption
    {
        public string SubscriptionId { get; set; }
        public bool IsSubscribed { get; set; }
        public string Name { get; set; }
    }
    public class ToolboxViewModel
    {
        public IEnumerable<ToolSubscriptionOption> Tools { get; set; }
    }
}