using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Abide.Annotations;
using Abide.RecordProviders;

namespace Abide
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly IByteConverter byteConverter = new ByteConverter();
        private readonly IndexManager indexManager;

        private bool databaseCreated;
        private readonly FileSystemScanner fileSystemScanner;

        private string indexOn;

        private string output;


        public ViewModel()
        {
            CreateDatabaseCommand = new DelegateCommand(CreateDatabase);
            CreateIndexCommand = new DelegateCommand(() => indexManager.CreateIndex("Practice"));
            WhereQueryCommand = new DelegateCommand(IndexedQuery);
            SequentialWhereQueryCommand = new DelegateCommand(SequentialQuery);
            CountQueryCommand = new DelegateCommand(() => CountQuery("London"));
            PeppermintQueryCommand = new DelegateCommand(() => AverageQuery("Peppermint Oil"));
            PostcodesQueryCommand = new DelegateCommand(PostcodeQuery);
            RegionAverageQueryCommand = new DelegateCommand(RegionQuery);
            fileSystemScanner = new FileSystemScanner();
            indexManager = new IndexManager(fileSystemScanner);

            DatabaseCreated = fileSystemScanner.DatabaseExists("orders");
            IndexOn = String.Join(", ", indexManager.AvailableIndices());
        }

        public ICommand CreateDatabaseCommand { get; set; }

        public ICommand CreateIndexCommand { get; set; }
        public ICommand WhereQueryCommand { get; set; }

        public bool DatabaseCreated
        {
            get { return databaseCreated; }
            set
            {
                if (value == databaseCreated) return;
                databaseCreated = value;
                OnPropertyChanged();
            }
        }

        public string IndexOn
        {
            get { return indexOn; }
            set
            {
                if (value == indexOn) return;
                indexOn = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegionAverageQueryCommand { get; set; }

        public ICommand PostcodesQueryCommand { get; set; }

        public ICommand PeppermintQueryCommand { get; set; }

        public ICommand CountQueryCommand { get; set; }

        public ICommand SequentialWhereQueryCommand { get; set; }

        public string Output
        {
            get { return output; }
            set
            {
                if (value == output) return;
                output = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private async Task<string> GetFormattedResultAsync(ResultSetPrinter printer)
        {
            var task = new Task<string>(printer.GetFormattedResults);
            task.Start();
            return await task;
        }

        private async Task<IEnumerable<IDictionary<string, dynamic>>> GetUnformattedResultAsync(RecordParser parser)
        {
            var task = new Task<IEnumerable<IDictionary<string, dynamic>>>(parser.ParseData);
            task.Start();
            return await task;
        }

        private async void RegionQuery()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var averageQuery = new RecordParser(
                new UngroupedAverageProvider("cost",
                    new SelectionRecordProvider(new[] {"cost", "bnf_name"},
                        new OrderDatabaseReader("orders.dat", indexManager, QueryMode.Auto,
                            new List<IWhereQueryConstraint>
                            {
                                new ContainsQueryConstraint("bnf_name", "Flucloxacillin"),
                                new Not(new ContainsQueryConstraint("bnf_name", "Co-Fluampicil")),
                                new Not(new EqualityConstraint("bnf_name", "", byteConverter))
                            }))));
            float avg =  (float) (await GetUnformattedResultAsync(averageQuery)).First()["avg_of_cost"];
            Func<RecordMetaData, byte[], dynamic> differ = (recordMetaData, row) =>
            {
                float value = BitConverter.ToSingle(row, recordMetaData.ColumnDescriptors["avg_of_cost"].Offset);
                return value - avg;
            };
            var query =
                new ResultSetPrinter(
                    new RecordParser(
                        new ComputedFieldProvider("diff", ColumnType.Float, differ,
                            new AverageRecordProvider("region", "cost",
                                new WhereRecordProvider(
                                    new JoinRecordProvider(
                                        new SelectionRecordProvider(new List<string> {"cost", "practice", "bnf_name"},
                                            new OrderDatabaseReader("orders.dat", indexManager, QueryMode.Auto,
                                                new List<IWhereQueryConstraint>())),
                                        new SelectionRecordProvider(new List<string> {"id", "region"},
                                            new PracticeDatabaseReader("practices.dat", indexManager,
                                                new List<IWhereQueryConstraint>())),
                                        "practice",
                                        "id"),
                                    new List<IWhereQueryConstraint>
                                    {
                                        new ContainsQueryConstraint("bnf_name", "Flucloxacillin"),
                                        new Not(new ContainsQueryConstraint("bnf_name", "Co-Fluampicil")),
                                        new Not(new EqualityConstraint("bnf_name", "", byteConverter))
                                    })))));
            var results = await GetFormattedResultAsync(query);
            stopwatch.Stop();
            Output += $"{results}Time: {stopwatch.ElapsedMilliseconds} ms\n";
        }

        private async void PostcodeQuery()
        {
            var query =
                new ResultSetPrinter(
                    new RecordParser(
                        new LimitRecordProvider(5,
                            new SortingRecordProvider("cost",
                                new SumRecordProvider("postal_code", "cost",
                                    new SelectionRecordProvider(new[] {"cost", "postal_code"},
                                        new JoinRecordProvider(
                                            new SelectionRecordProvider(new List<string> {"cost", "practice"},
                                                new OrderDatabaseReader("orders.dat", indexManager, QueryMode.Auto,
                                                    new List<IWhereQueryConstraint>())),
                                            new SelectionRecordProvider(new List<string> {"id", "postal_code"},
                                                new PracticeDatabaseReader("practices.dat", indexManager,
                                                    new List<IWhereQueryConstraint>())),
                                            "practice",
                                            "id"))),
                                SortingRecordProvider.Order.DESCENDING
                                ))));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var results = await GetFormattedResultAsync(query);
            stopwatch.Stop();
            Output += $"{results}Time: {stopwatch.ElapsedMilliseconds} ms\n";
        }

        private async void AverageQuery(string product)
        {
            var query =
                new ResultSetPrinter(
                    new RecordParser(
                        new WhereRecordProvider(
                            new AverageRecordProvider("bnf_name", "cost",
                                new OrderDatabaseReader("orders.dat", indexManager, QueryMode.Auto,
                                    new List<IWhereQueryConstraint>())),
                            new[] {new EqualityConstraint("bnf_name", "Peppermint Oil", byteConverter)})));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var results = await GetFormattedResultAsync(query);
            stopwatch.Stop();
            Output += $"{results}Time: {stopwatch.ElapsedMilliseconds} ms\n";
        }

        private async void SequentialQuery()
        {
            var stopwatch = new Stopwatch();
            var query = new RecordParser( new OrderDatabaseReader("orders.dat", indexManager, QueryMode.Sequential,
                new List<IWhereQueryConstraint>
                {
                    new StringEqualsConstraint("practice", "A86001", byteConverter)
                }));
            stopwatch.Start();
            var results = await GetUnformattedResultAsync(query);
            stopwatch.Stop();
            Output += $"Count: {results.Count()}, Time: {stopwatch.ElapsedMilliseconds} ms\n";
        }

        private async void IndexedQuery()
        {
            var stopwatch = new Stopwatch();
            var reader = new RecordParser(new OrderDatabaseReader("orders.dat", indexManager, QueryMode.Auto,
                new List<IWhereQueryConstraint>
                {
                    new StringEqualsConstraint("Practice", "A86001", byteConverter)
                }));
            stopwatch.Start();
            var results = await GetUnformattedResultAsync(reader);
            stopwatch.Stop();
            Output += $"Count: {results.Count()}, Time: {stopwatch.ElapsedMilliseconds} ms\n";
        }

        private async void CountQuery(string city)
        {
            var query =
                new ResultSetPrinter(
                    new RecordParser(
                        new CountRecordProvider("city", "id",
                            new PracticeDatabaseReader("practices.dat", indexManager,
                                new List<IWhereQueryConstraint>
                                {
                                    new EqualityConstraint("city", "LONDON", byteConverter)
                                }))));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var results = await GetFormattedResultAsync(query);
            stopwatch.Stop();
            Output += $"{results}Time: {stopwatch.ElapsedMilliseconds} ms\n";
        }


        private async void CreateDatabase()
        {
            var task = new Task(() =>
            {
                var lines = File.ReadAllLines("T201109PDP IEXT.CSV");
                using (var writer = new BinaryWriter(new FileStream("orders.dat", FileMode.Create)))
                {
                    foreach (var line in lines.Skip(1))
                    {
                        var columns = line.Split(',');
                        columns = columns.Select(c => c.TrimEnd(' ')).ToArray();
                        writer.Write(Encoding.ASCII.GetBytes(columns[0].PadRight(4, '\0')));
                        writer.Write(Encoding.ASCII.GetBytes(columns[1].PadRight(4, '\0')));
                        writer.Write(Encoding.ASCII.GetBytes(columns[2].PadRight(6, '\0')));
                        writer.Write(Encoding.ASCII.GetBytes(columns[3].PadRight(10, '\0')));
                        writer.Write(Encoding.ASCII.GetBytes(columns[4].PadRight(40, '\0')));
                        writer.Write(int.Parse(columns[5]));
                        writer.Write(float.Parse(columns[6]));
                        writer.Write(float.Parse(columns[7]));
                        writer.Write(int.Parse(columns[8]));
                    }
                }
                lines = null;
                lines = File.ReadAllLines("T201202ADD REXT.CSV");
                using (var writer = new BinaryWriter(new FileStream("practices.dat", FileMode.Create)))
                {
                    foreach (var line in lines.Skip(1))
                    {
                        var columns = line.Split(',');
                        columns = columns.Select(c => c.TrimEnd(' ')).ToArray();
                        writer.Write(int.Parse(columns[0]));
                        writer.Write(Encoding.ASCII.GetBytes(columns[1].PadRight(6, '\0')));
                        writer.Write(Encoding.ASCII.GetBytes(columns[2].PadRight(40, '\0')));
                        writer.Write(Encoding.ASCII.GetBytes(columns[3].PadRight(40, '\0')));
                        writer.Write(Encoding.ASCII.GetBytes(columns[4].PadRight(40, '\0')));
                        writer.Write(Encoding.ASCII.GetBytes(columns[5].PadRight(40, '\0')));
                        writer.Write(Encoding.ASCII.GetBytes(columns[6].PadRight(40, '\0')));
                        writer.Write(Encoding.ASCII.GetBytes(columns[7].PadRight(10, '\0')));
                    }
                }
            });
            task.Start();
            await task;
            DatabaseCreated = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum QueryMode
    {
        Sequential,
        Auto
    }
}