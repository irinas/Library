using Library.Service.Utilities;
using Xunit;
namespace Library.UnitTests;

public sealed class BookExercisesTests
{
    [Theory][InlineData(1, true)][InlineData(2, true)][InlineData(16, true)][InlineData(0, false)][InlineData(-2, false)][InlineData(3, false)][InlineData(long.MaxValue, false)] public void IsPowerOfTwo_HandlesBoundaries(long value, bool expected) => Assert.Equal(expected, BookExercises.IsPowerOfTwo(value));
    [Theory][InlineData("Moby Dick", "kciD yboM")][InlineData("", " ")][InlineData("a", "a")] public void ReverseTitle_ReversesCharacters(string input, string expected) { if (input == "") expected = ""; Assert.Equal(expected, BookExercises.ReverseTitle(input)); }
    [Fact] public void ReverseTitle_NullThrows() => Assert.Throws<ArgumentNullException>(() => BookExercises.ReverseTitle(null!));
    [Theory][InlineData("Read", 3, "ReadReadRead")][InlineData("x", 0, "")][InlineData("", 5, "")] public void ReplicateTitle_Repeats(string title, int count, string expected) => Assert.Equal(expected, BookExercises.ReplicateTitle(title, count));
    [Fact] public void ReplicateTitle_NegativeCountThrows() => Assert.Throws<ArgumentOutOfRangeException>(() => BookExercises.ReplicateTitle("x", -1));
    [Fact] public void OddBookIds_ReturnsEveryOddNumberFromOneToNinetyNine() { var ids = BookExercises.OddBookIds(); Assert.Equal(50, ids.Count); Assert.Equal(Enumerable.Range(1, 50).Select(x => x * 2 - 1), ids); }
}
