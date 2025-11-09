using System;

namespace LockSim
{
    [Serializable]
    public struct Contact
    {
        public int BodyAId;
        public int BodyBId;
        public FPVector2 Position;
        public FPVector2 Normal; // Points from A to B
        public FP Penetration;

        public Contact(int bodyAId, int bodyBId, FPVector2 position, FPVector2 normal, FP penetration)
        {
            BodyAId = bodyAId;
            BodyBId = bodyBId;
            Position = position;
            Normal = normal;
            Penetration = penetration;
        }
    }

    [Serializable]
    public struct ContactManifold
    {
        public int BodyAId;
        public int BodyBId;
        public FPVector2 Normal; // Points from A to B
        public FP Penetration;
        public int ContactCount;
        
        // Support up to 2 contact points (common for box-box)
        public FPVector2 ContactPoint1;
        public FPVector2 ContactPoint2;

        public ContactManifold(int bodyAId, int bodyBId)
        {
            BodyAId = bodyAId;
            BodyBId = bodyBId;
            Normal = FPVector2.Zero;
            Penetration = FP.Zero;
            ContactCount = 0;
            ContactPoint1 = FPVector2.Zero;
            ContactPoint2 = FPVector2.Zero;
        }

        public void AddContact(FPVector2 point)
        {
            if (ContactCount == 0)
            {
                ContactPoint1 = point;
                ContactCount = 1;
            }
            else if (ContactCount == 1)
            {
                // Only add if sufficiently far from first point
                FP distSqr = FPVector2.SqrDistance(ContactPoint1, point);
                if (distSqr > FP.FromFloat(0.01f))
                {
                    ContactPoint2 = point;
                    ContactCount = 2;
                }
            }
        }
    }
}

