using KYC360_InternshipAssessment.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetryTests
{
    [TestClass]
    public class RetryTests
    {
        [TestMethod]
        public async Task DatabaseWriteOperationSucceedsAfterRetries()
        {
            int attemptCount = 0;

            var result = await Retry.ExecuteAsync(
                () =>
                {
                    attemptCount++;
                    if (attemptCount <= 3)
                    {
                        throw new Exception("Database write failed");
                    }
                    return Task.FromResult("Database Write successful");
                },
                (result) => result == "Database Write successful"

            );

            Assert.AreEqual("Database Write successful", result);
        }

        [TestMethod]
        public async Task DatabaseWriteOperationFailsAfterMaxRetries()
        {
            bool exceptionThrown = false;

            try
            {
                await Retry.ExecuteAsync<string>(
                     () =>
                    {
                        throw new Exception("Database write failed");
                    },
                    maxRetries: 3
                );
            }
            catch (AggregateException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }
    }
}