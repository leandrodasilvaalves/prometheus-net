namespace Api.Prometheus;

public class Simulator
{
    public static Task Delay(int min = 500, int max = 5000) =>
        Task.Delay(TimeSpan.FromMicroseconds(Random.Shared.Next(min, max)));
}
