using ECommerce.Models;

namespace Delegates
{
	public delegate void OrderProcessedEventHandler(Order o, string status);
}
