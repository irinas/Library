namespace Library.Service.Utilities;
/// <summary>Small assignment warm-ups, kept pure and independently testable.</summary>
public static class BookExercises
{
 public static bool IsPowerOfTwo(long bookId) => bookId > 0 && (bookId & (bookId - 1)) == 0;
 public static string ReverseTitle(string title) { ArgumentNullException.ThrowIfNull(title); return new string(title.Reverse().ToArray()); }
 public static string ReplicateTitle(string title,int count) { ArgumentNullException.ThrowIfNull(title); if(count<0) throw new ArgumentOutOfRangeException(nameof(count)); if((long)title.Length*count>1_000_000) throw new ArgumentOutOfRangeException(nameof(count),"Replicated title exceeds the 1,000,000 character safety limit."); return string.Concat(Enumerable.Repeat(title,count)); }
 public static IReadOnlyList<int> OddBookIds() => Enumerable.Range(1,50).Select(x=>x*2-1).ToArray();
}
