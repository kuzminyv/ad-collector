using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.DAL;
using Core.Entities;
using Newtonsoft.Json;
using MsSql = Core.DAL.MsSql;
using Bin = Core.DAL.Binary;
using System.Threading;
using Core.BLL.Common;
using System.Threading.Tasks;
using Core.DAL.API;
using Core.DAL.Common;

namespace Core.BLL
{
    public class ExportManager
    {
        public void ExportToJson(string foolder)
        {
            File.WriteAllText(Path.Combine(foolder, "Ad.json"),
                JsonConvert.SerializeObject(Repositories.AdsRepository.GetList(null).Items.ToArray(), Formatting.Indented));
            File.WriteAllText(Path.Combine(foolder, "AdLinks.json"),
                JsonConvert.SerializeObject(Repositories.AdLinksRepository.GetList(null).Items.ToArray(), Formatting.Indented));
            File.WriteAllText(Path.Combine(foolder, "LogEntries.json"),
                JsonConvert.SerializeObject(Repositories.LogEntriesRepository.GetList(null).Items.ToArray(), Formatting.Indented));
        }

        public CancellationTokenSource ExportToBinaryAsync(Action<OperationState> stateChangedCallback, Action completedCallback)
        {
            return TaskHelper.ExecuteAsync(ExportToBinary, stateChangedCallback, completedCallback);
        }

        private  void ExportToBinary(Action<OperationState> stateChangedCallback, Action completedCallback, CancellationToken cancelationToken)
        {
            Export<Ad>(stateChangedCallback, cancelationToken,
                    (start, limit) => new MsSql.AdsRepository().GetList(new Query(start, limit)), new Bin.AdsRepository(), 1000);

            Export<AdImage>(stateChangedCallback, cancelationToken,
                 (start, limit) => new MsSql.AdImagesRepository().GetList(new Query(start, limit)), new Bin.AdImagesRepository(), 1000);

            Export<AdHistoryItem>(stateChangedCallback, cancelationToken,
                 (start, limit) => new MsSql.AdHistoryItemsRepository().GetList(new Query(start, limit)), new Bin.AdHistoryItemsRepository(), 1000);
            completedCallback();
        }

        public CancellationTokenSource ExportToSqlAsync(Action<OperationState> stateChangedCallback, Action completedCallback)
        {
            return TaskHelper.ExecuteAsync(ExportToSql, stateChangedCallback, completedCallback);
        }

        private void Export<TEntity>(Action<OperationState> stateChangedCallback, CancellationToken cancelationToken,
            Func<int, int, QueryResult<TEntity>> sourceEntities, IRepository<TEntity> targetRepository, int bufferSize)
        {
            int total = sourceEntities(0, 1).TotalCount.Value; 

            OperationState state = new OperationState();

            state.Progress = 0;
            state.ProgressTotal = total;
            state.Description = typeof(TEntity).Name;
            stateChangedCallback(state);

            

            if (bufferSize == 1)
            {
                for (int i = 0; i < total; i++)
                {
                    if (cancelationToken.IsCancellationRequested)
                    {
                        state.Canceled = true;
                        stateChangedCallback(state);
                        break;
                    }
                    targetRepository.AddItem(sourceEntities(i, bufferSize).Items[0]);
                    state.Progress++;
                    stateChangedCallback(state);
                }
            }
            else
            {
                for (int i = 0; i <= total / bufferSize; i++)
                {
                    if (cancelationToken.IsCancellationRequested)
                    {
                        state.Canceled = true;
                        stateChangedCallback(state);
                        break;
                    }
                    var entitiesToAdd = sourceEntities(i * bufferSize, bufferSize).Items;
                    targetRepository.AddList(entitiesToAdd);
                    state.Progress += entitiesToAdd.Count();
                    stateChangedCallback(state);
                }
            }
        }

        private void ExportToSql(Action<OperationState> stateChangedCallback, Action completedCallback, CancellationToken cancelationToken)
        {
            var ads = new Bin.AdsRepository().GetList(null).Items;
            var histories = new Bin.AdHistoryItemsRepository().GetList(null).Items;

            var adIds = ads.Select(a => a.Id).ToList();

            Export<Ad>(stateChangedCallback, cancelationToken,
               (start, limit) => new QueryResult<Ad>(ads.Skip(start).Take(limit).ToList(), ads.Count) , new MsSql.AdsRepository(), 1);

            var adIdMap = new Dictionary<int, int>();
            for (int i = 0; i < adIds.Count(); i++)
            {
                adIdMap.Add(adIds[i], ads[i].Id);
            }
            foreach (var historyItem in histories)
            {
                historyItem.AdId = adIdMap[historyItem.AdId];
            }

            Export<AdHistoryItem>(stateChangedCallback, cancelationToken,
                (start, limit) => new QueryResult<AdHistoryItem>(histories.Skip(start).Take(limit).ToList(), histories.Count), new MsSql.AdHistoryItemsRepository(), 100);

            completedCallback();
        }
    }
}
