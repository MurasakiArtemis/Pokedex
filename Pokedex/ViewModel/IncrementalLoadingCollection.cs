using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Pokedex.ViewModel
{
    public class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        private int MaxItems { get; set; }
        Func<long, long, IEnumerable<T>> GetElementsList { get; set; }
        Func<T, Task<T>> LoadIndividualItem { get; set; }
        public IncrementalLoadingCollection(int maxItems, Func<long, long, IEnumerable<T>> getElementsList, Func<T, Task<T>> loadIndividualItem)
        {
            MaxItems = maxItems;
            GetElementsList = getElementsList;
            LoadIndividualItem = loadIndividualItem;
        }
        public bool HasMoreItems { get { return Count < MaxItems; } }
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return InnerLoadMoreItemsAsync(count).AsAsyncOperation();
        }
        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint expectedCount)
        {
            var dataList = GetElementsList(Count, Count + expectedCount);
            int startingCount = Count;
            try
            {
                if (dataList != null && dataList.Any())
                    foreach (var element in dataList)
                        Add(await LoadIndividualItem(element));
                else
                    throw new Exception();
            }
            catch
            {
                MaxItems = Count;
            }
            return new LoadMoreItemsResult { Count = (uint)(Count - startingCount) };
        }
    }
}
