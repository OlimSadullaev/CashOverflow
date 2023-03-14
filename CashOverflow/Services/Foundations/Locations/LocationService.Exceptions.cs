// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by CashOverflow Team
// --------------------------------------------------------

using System.Threading.Tasks;
using CashOverflow.Models.Locations;
using CashOverflow.Models.Locations.Exceptions;
using Microsoft.Data.SqlClient;
using Xeptions;

namespace CashOverflow.Services.Foundations.Locations
{
    public partial class LocationService
    {
        private delegate ValueTask<Location> ReturningLocationFunction();

        private async ValueTask<Location> TryCatch(ReturningLocationFunction returningLocationFunction)
        {
            try
            {
                return await returningLocationFunction();
            }
            catch (InvalidLocationException invalidLocationException)
            {
                throw CreateAndLogValidationsException(invalidLocationException);
            }
            catch (NotFoundLocationException notFoundLocationException)
            {
                throw CreateAndLogValidationsException(notFoundLocationException);
            }
            catch(SqlException sqlException)
            {
                var failedLocationStoragException = new FailedLocationStorageException(sqlException);

                throw CreateAndLogDependencyException(failedLocationStoragException);
            }
        }

        private LocationValidationException CreateAndLogValidationsException(Xeption exception)
        {
            var locationValidationException = new LocationValidationException(exception);
            this.loggingBroker.LogError(locationValidationException);

            return locationValidationException;
        }

        private LocationDependencyException CreateAndLogDependencyException(Xeption xeption)
        {
            var locationDependencyException = new LocationDependencyException(xeption);
            this.loggingBroker.LogCritical(locationDependencyException);

            return locationDependencyException;
        }
    }
}