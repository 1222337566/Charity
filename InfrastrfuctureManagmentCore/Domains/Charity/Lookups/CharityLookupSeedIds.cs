namespace InfrastrfuctureManagmentCore.Domains.Charity.Lookups
{
    public static class CharityLookupSeedIds
    {
        public static readonly Guid GenderMale = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid GenderFemale = Guid.Parse("22222222-2222-2222-2222-222222222222");

        public static readonly Guid MaritalSingle = Guid.Parse("33333333-3333-3333-3333-333333333331");
        public static readonly Guid MaritalMarried = Guid.Parse("33333333-3333-3333-3333-333333333332");
        public static readonly Guid MaritalWidowed = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public static readonly Guid MaritalDivorced = Guid.Parse("33333333-3333-3333-3333-333333333334");

        public static readonly Guid BeneficiaryStatusNew = Guid.Parse("44444444-4444-4444-4444-444444444441");
        public static readonly Guid BeneficiaryStatusUnderReview = Guid.Parse("44444444-4444-4444-4444-444444444442");
        public static readonly Guid BeneficiaryStatusApproved = Guid.Parse("44444444-4444-4444-4444-444444444443");
        public static readonly Guid BeneficiaryStatusSuspended = Guid.Parse("44444444-4444-4444-4444-444444444444");
        public static readonly Guid BeneficiaryStatusRejected = Guid.Parse("44444444-4444-4444-4444-444444444445");

        public static readonly Guid AidTypeCash = Guid.Parse("55555555-5555-5555-5555-555555555551");
        public static readonly Guid AidTypeFood = Guid.Parse("55555555-5555-5555-5555-555555555552");
        public static readonly Guid AidTypeMedical = Guid.Parse("55555555-5555-5555-5555-555555555553");
        public static readonly Guid AidTypeEducational = Guid.Parse("55555555-5555-5555-5555-555555555554");
        public static readonly Guid AidTypeClothes = Guid.Parse("55555555-5555-5555-5555-555555555555");
        public static readonly Guid AidTypeDevices = Guid.Parse("55555555-5555-5555-5555-555555555556");
        public static readonly Guid AidTypeSponsorship = Guid.Parse("55555555-5555-5555-5555-555555555557");
    }
}
