using ConsoleApp.Model;
using ConsoleApp.Model.Enum;
using ConsoleApp.OutputTypes;
using System.Data.Common;
using System.Linq;

namespace ConsoleApp;

public class QueryHelper : IQueryHelper
{
    /// <summary>
    /// Get Deliveries that has payed
    /// </summary>
    public IEnumerable<Delivery> Paid(IEnumerable<Delivery> deliveries)
    {
        return deliveries.Where(d => d.PaymentId != null);
    } //TODO: Завдання 1

    /// <summary>
    /// Get Deliveries that now processing by system (not Canceled or Done)
    /// </summary>
    public IEnumerable<Delivery> NotFinished(IEnumerable<Delivery> deliveries) 
    {
        return deliveries.Where(d => d.Status != DeliveryStatus.Cancelled && d.Status != DeliveryStatus.Done);
    } //TODO: Завдання 2
    
    /// <summary>
    /// Get DeliveriesShortInfo from deliveries of specified client
    /// </summary>
    public IEnumerable<DeliveryShortInfo> DeliveryInfosByClient(IEnumerable<Delivery> deliveries, string clientId)
    {
        return deliveries.Where(d => d.ClientId == clientId).Select(s => new DeliveryShortInfo
        {
                Id = s.Id,
                StartCity = s.Direction.Origin.City,
                EndCity = s.Direction.Destination.City,
                ClientId = s.ClientId,
                Type = s.Type,
                LoadingPeriod = s.LoadingPeriod,
                ArrivalPeriod = s.ArrivalPeriod,
                Status = s.Status,
                CargoType = s.CargoType
        });
    } //TODO: Завдання 3
    
    /// <summary>
    /// Get first ten Deliveries that starts at specified city and have specified type
    /// </summary>
    public IEnumerable<Delivery> DeliveriesByCityAndType(IEnumerable<Delivery> deliveries, string cityName, DeliveryType type)
    {
        return deliveries.Where(d => d.Direction.Origin.City == cityName && d.Type == type).Take(10).Select(s => new Delivery
        {
                Id = s.Id,
                Direction = s.Direction,
                ClientId = s.ClientId,
                Type = s.Type,
                PaymentId = s.PaymentId,
                LoadingPeriod = s.LoadingPeriod,
                ArrivalPeriod = s.ArrivalPeriod,
                Status = s.Status,
                CargoType = s.CargoType
        });
    } //TODO: Завдання 4
    
    /// <summary>
    /// Order deliveries by status, then by start of loading period
    /// </summary>
    public IEnumerable<Delivery> OrderByStatusThenByStartLoading(IEnumerable<Delivery> deliveries) 
    {
        return deliveries.OrderBy(d => d.Status).ThenBy(d => d.LoadingPeriod.Start).Select(s => new Delivery
        {
                Id = s.Id,
                Direction = s.Direction,
                ClientId = s.ClientId,
                Type = s.Type,
                PaymentId = s.PaymentId,
                LoadingPeriod = s.LoadingPeriod,
                ArrivalPeriod = s.ArrivalPeriod,
                Status = s.Status,
                CargoType = s.CargoType
        });
    } //TODO: Завдання 5

    /// <summary>
    /// Count unique cargo types
    /// </summary>
    public int CountUniqCargoTypes(IEnumerable<Delivery> deliveries)
    {
        return deliveries.Select(d => d.CargoType).Distinct().Count();
    } //TODO: Завдання 6
    
    /// <summary>
    /// Group deliveries by status and count deliveries in each group
    /// </summary>
    public Dictionary<DeliveryStatus, int> CountsByDeliveryStatus(IEnumerable<Delivery> deliveries)
    {
        return deliveries.GroupBy(d => d.Status).ToDictionary(g => g.Key, g => g.Count());
    } //TODO: Завдання 7
    
    /// <summary>
    /// Group deliveries by start-end city pairs and calculate average gap between end of loading period and start of arrival period (calculate in minutes)
    /// </summary>
    public IEnumerable<AverageGapsInfo> AverageTravelTimePerDirection(IEnumerable<Delivery> deliveries)
    {
        return deliveries.GroupBy(d => new 
        { 
            StartCity = d.Direction.Origin.City, 
            EndCity = d.Direction.Destination.City 
        })
        .Select(g => new AverageGapsInfo
        {
            StartCity = g.Key.StartCity,
            EndCity = g.Key.EndCity,
            AverageGap = g.Where(d => d.ArrivalPeriod.Start != null && d.LoadingPeriod.End != null)
                         .Average(d => (d.ArrivalPeriod.Start - d.LoadingPeriod.End)?.TotalMinutes ?? 0)
        });
    } //TODO: Завдання 8

    /// <summary>
    /// Paging helper
    /// </summary>
    public IEnumerable<TElement> Paging<TElement, TOrderingKey>(IEnumerable<TElement> elements,
        Func<TElement, TOrderingKey> ordering,
        Func<TElement, bool>? filter = null,
        int countOnPage = 100,
        int pageNumber = 1) 
        {
            return elements.Where(filter ?? (e => true))
                .OrderBy(ordering)
                .Skip(countOnPage * (pageNumber - 1))
                .Take(countOnPage);
        } //TODO: Завдання 9 
}