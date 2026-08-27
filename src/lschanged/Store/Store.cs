using LsChanged.Exceptions;
using LsChanged.Store.Abstractions;

namespace LsChanged.Store;

internal sealed class Store : IStore
{
    private const string _markerFileName = ".lschanged.store.marker";

    private static readonly Func<string, IStoreEntry> _storeEntryFactory = x => new StoreEntry(x);

    private readonly string _pathToStore;
    private readonly ICurrentTimeProvider _timeProvider;
    private readonly IStoreRecordReader _reader;
    private readonly IStoreRecordWriter _writer;

    public Store(string pathToStore,
                 ICurrentTimeProvider timeProvider,
                 IStoreRecordReader reader,
                 IStoreRecordWriter writer)
    {
        ArgumentException.ThrowIfNullOrEmpty(pathToStore);
        EnsureDirectoryAccessible(pathToStore);
        _pathToStore = pathToStore;

        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;

        ArgumentNullException.ThrowIfNull(reader);
        _reader = reader;

        ArgumentNullException.ThrowIfNull(writer);
        _writer = writer;

        InitializeStore();
    }

    private void InitializeStore()
    {
        string markerFilePath = Path.Combine(_pathToStore, _markerFileName);

        try
        {
            if (File.Exists(markerFilePath))
            {
                byte[] bytes = File.ReadAllBytes(markerFilePath);
                if (bytes == null || bytes.Length > 0)
                {
                    throw new InvalidOperationException("Invalid store marker file.");
                }

                return;
            }

            File.WriteAllBytes(markerFilePath, Array.Empty<byte>());
        }
        catch (Exception exception)
        {
            throw new StoreInaccessibleException(exception);
        }
        
    }

    public IEnumerable<IStoreEntry> ListAll()
    {
        var files = GetEntryFiles();

        var entries = files.Select(x => _storeEntryFactory(x)).ToList();
        return entries;
    }

    public IStoreRecord Get(IStoreEntry storeEntry)
    {
        ArgumentNullException.ThrowIfNull(storeEntry);

        var result = _reader.Read(storeEntry);
        return result;
    }

    public IStoreRecord? GetByOrdinal(int ordinal)
    {
        var storeEntries = ListAll();
        var storeEntry = storeEntries.Skip(ordinal).FirstOrDefault();

        var result = storeEntry != null ? Get(storeEntry) : null;
        return result;
    }

    public IStoreEntry Add(IStoreRecord storeRecord)
    {
        var entry = NewStoreEntry();
        
        var modifiedEntry = _writer.Write(entry, storeRecord);
        return modifiedEntry;
    }

    public void Clear()
    {
        var files = GetEntryFiles();
        foreach (var file in files)
        {
            File.Delete(file);
        }
    }

    public bool Delete(IStoreEntry storeEntry)
    {
        ArgumentNullException.ThrowIfNull(storeEntry);

        string filePath = storeEntry.Id;
        if (!File.Exists(filePath))
        {
            return false;
        }

        File.Delete(filePath);
        return true;
    }

    private static void EnsureDirectoryAccessible(string directory)
    {
        try
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var files = Directory.GetFiles(directory);
        }
        catch (Exception exception)
        {
            throw new StoreInaccessibleException(exception);
        }
    }

    private IEnumerable<string> GetEntryFiles()
    {
        var files = Directory.GetFiles(_pathToStore)
            .Where(x => Path.GetFileName(x) != _markerFileName);
        return files;
    }

    private IStoreEntry NewStoreEntry()
    {
        var now = _timeProvider.CurrentTime;
        string fileName = now.ToString("yyyy-MM-dd.HHmmss") + ".json";
        string filePath = Path.Combine(_pathToStore, fileName);

        var result = _storeEntryFactory(filePath);
        return result;
    }
}
