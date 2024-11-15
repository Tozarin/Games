using Database;
using Microsoft.EntityFrameworkCore;

namespace BlazorSolver.Components.Util;

public class Page<T> where T : class
{
    private IQueryable<T> _data;

    public int LastPageNum => LastNum / _size;
    private const int _size = 15;
    private int _lastNum;
    public int Size => _size;
    public int LastNum => _lastNum;

    public Page(IQueryable<T> data)
    {
        _data = data;
        _lastNum = data.Count();
    }

    public async Task<IEnumerable<T>> GetPage(int num)
    {

        if (num > LastPageNum || num < 0)
            throw new ArgumentOutOfRangeException();

        var data = await _data.Skip(num * _size).Take(_size).ToListAsync();

        return data;
    }
}
