﻿// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by CashOverflow Team
// --------------------------------------------------------

using System.Threading.Tasks;
using CashOverflow.Models.Jobs;
using CashOverflow.Models.Jobs.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace CashOverflow.Tests.Unit.Services.Foundations.Jobs
{
    public partial class JobServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfJobIsNullAndLogItAsync()
        {
            // given
            Job nullJob = null;
            var nullJobException = new NullJobException();
            
            var expectedJobValidationException =
                new JobValidationException(nullJobException);

            // when
            ValueTask<Job> modifyJobTask = this.jobService.ModifyJobAsync(nullJob);

            JobValidationException actualJobValidationException =
                await Assert.ThrowsAsync<JobValidationException>(
                    modifyJobTask.AsTask);

            // then
            actualJobValidationException.Should().BeEquivalentTo(expectedJobValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedJobValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfJobIsInvalidAndLogItAsync(string invalidString)
        {
            // given
            Job invalidJob = new Job
            {
                Title = invalidString
            };

            var invalidJobException = new InvalidJobException();

            invalidJobException.AddData(
                key: nameof(Job.Id),
                values: "Id is required");

            invalidJobException.AddData(
                key: nameof(Job.Title),
                values: "Title is required");

            invalidJobException.AddData(
                key: nameof(Job.CreatedDate),
                values: "Value is required");

            invalidJobException.AddData(
                key: nameof(Job.UpdatedDate),
                values: new[]
                    {
                        "Value is required",
                        "Date is not recent.",
                        $"Date is the same as {nameof(Job.CreatedDate)}"
                    }
                );

            var expectedJobValidationException =
                new JobValidationException(invalidJobException);


            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Returns(GetRandomDateTime);

            // when
            ValueTask<Job> modifyJobTask = this.jobService.ModifyJobAsync(invalidJob);

            JobValidationException actualJobValidationException =
                await Assert.ThrowsAsync<JobValidationException>(
                    modifyJobTask.AsTask);

            // then
            actualJobValidationException.Should().BeEquivalentTo(expectedJobValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedJobValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
