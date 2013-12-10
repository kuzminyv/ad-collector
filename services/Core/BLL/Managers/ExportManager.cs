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
            var ads = new MsSql.AdsRepository().GetList(null).Items;
            var histories = new MsSql.AdHistoryItemsRepository().GetList(null).Items;

            Export<Ad>(stateChangedCallback, cancelationToken,
                    ads, new Bin.AdsRepository(), ads.Count);

            Export<AdHistoryItem>(stateChangedCallback, cancelationToken,
                histories, new Bin.AdHistoryItemsRepository(), histories.Count);
            completedCallback();
        }

        public CancellationTokenSource ExportToSqlAsync(Action<OperationState> stateChangedCallback, Action completedCallback)
        {
            return TaskHelper.ExecuteAsync(ExportToSql, stateChangedCallback, completedCallback);
        }

        private void Export<TEntity>(Action<OperationState> stateChangedCallback, CancellationToken cancelationToken,
            List<TEntity> sourceEntities, IRepository<TEntity> targetRepository, int bufferSize)
        {
            OperationState state = new OperationState();

            state.Progress = 0;
            state.ProgressTotal = sourceEntities.Count();
            state.Description = typeof(TEntity).Name;
            stateChangedCallback(state);

            if (bufferSize == 1)
            {
                for (int i = 0; i < sourceEntities.Count(); i++)
                {
                    if (cancelationToken.IsCancellationRequested)
                    {
                        state.Canceled = true;
                        stateChangedCallback(state);
                        break;
                    }
                    targetRepository.AddItem(sourceEntities[i]);
                    state.Progress++;
                    stateChangedCallback(state);
                }
            }
            else
            {
                for (int i = 0; i <= sourceEntities.Count / bufferSize; i++)
                {
                    if (cancelationToken.IsCancellationRequested)
                    {
                        state.Canceled = true;
                        stateChangedCallback(state);
                        break;
                    }
                    var entitiesToAdd = sourceEntities.Skip(i * bufferSize).Take(bufferSize).ToList();
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
               ads, new MsSql.AdsRepository(), 1);

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
                histories, new MsSql.AdHistoryItemsRepository(), 100);

            completedCallback();
        }
    }
}
