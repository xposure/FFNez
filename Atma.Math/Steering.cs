using System;

namespace Atma
{
    public struct Steering
    {
        private static Random random = new Random();

        public vec2 steering;

        public vec2 integrate(vec2 p, vec2 velocity, float maxForce, float maxVelocity, float mass)
        {
            //if (steering == Vector2.Zero)
            //{
            //    velocity *= drag;
            //    if (Math.Abs(velocity.X) < 0.1f)
            //        velocity.X = 0f;

            //    if (Math.Abs(velocity.Y) < 0.1f)
            //        velocity.Y = 0f;
            //}
            //else
            steering = truncate(steering, maxForce);
            steering /= mass;
            velocity = truncate(velocity + steering, maxVelocity);

            return velocity;
        }

        public static Steering zero { get { return new Steering() { steering = vec2.Zero }; } }

        public static vec2 truncate(vec2 p, float len)
        {
            var i = len / p.Length;
            i = i < 1.0f ? i : 1.0f;

            return p * i;
        }

        public static Steering seek(float maxVelocity, vec2 from, vec2 target)
        {
            return new Steering() { steering = (target - from).Normalized * maxVelocity };
            //steering += (target - from).ToNormalized() * maxVelocity * influence;
        }

        public static Steering arrive(float maxVelocity, vec2 currentVelocity, vec2 from, vec2 target, float radius)
        {
            return arrive(maxVelocity, currentVelocity, from, target, radius, 1f);
        }

        public static Steering arrive(float maxVelocity, vec2 currentVelocity, vec2 from, vec2 target, float radius, float stopModifier)
        {
            var direction = (target - from).Normalized;
            var desiredVelocity = direction * maxVelocity;

            var distance = (target - from).Length;

            if (distance < radius)
                desiredVelocity *= (distance / radius) / stopModifier;

            return new Steering { steering = (desiredVelocity - currentVelocity) };
        }

        public static Steering flee(float maxVelocity, vec2 currentVelocity, vec2 from, vec2 target)
        {
            var direction = (from - target).Normalized;
            var desiredVelocity = direction * maxVelocity;

            return new Steering() { steering = (desiredVelocity - currentVelocity) };
        }

        public static Steering wander(float maxVelocity, float maxForce, vec2 currentVelocity, float currentOrientation, vec2 from)
        {
            return wander(maxVelocity, maxForce, currentVelocity, currentOrientation, from, 6f, 8f, 1f);
        }

        public static Steering wander(float maxVelocity, float maxForce, vec2 currentVelocity, float currentOrientation, vec2 from, float distance, float radius, float angleChange)
        {
            var circleCenter = currentVelocity.Normalized * distance;
            if (circleCenter == vec2.Zero)
            {
                //currentOrientation = (float)random.NextDouble() * Utility.TwoPi;
                circleCenter = new vec2(glm.Cos(currentOrientation), glm.Sin(currentOrientation)) * distance;
            }

            currentOrientation += (float)random.NextDouble() * angleChange - angleChange * 0.5f;
            var displacement = new vec2(glm.Cos(currentOrientation), glm.Sin(currentOrientation)) * radius;

            displacement = truncate(circleCenter + displacement, maxForce);
            return seek(maxVelocity, from, from + displacement);
        }

        public static Steering pursue(float maxVelocity, vec2 from, vec2 target, vec2 targetVelocity)
        {
            var distance = (from - target);
            var t = distance.Length / maxVelocity;
            return seek(maxVelocity, from, target + t * targetVelocity);
        }

        public static Steering evade(float maxVelocity, vec2 currentVelocity, vec2 from, vec2 target, vec2 targetVelocity)
        {
            var distance = (from - target);
            var t = distance.Length / maxVelocity;
            return flee(maxVelocity, currentVelocity, from, target + t * targetVelocity);
        }

        public static Steering operator +(Steering a, Steering b)
        {
            return new Steering { steering = a.steering + b.steering };
        }

        public static Steering operator -(Steering a, Steering b)
        {
            return new Steering { steering = a.steering - b.steering };
        }

        public static Steering operator *(Steering a, Steering b)
        {
            return new Steering { steering = a.steering * b.steering };
        }

        public static Steering operator /(Steering a, Steering b)
        {
            return new Steering { steering = a.steering / b.steering };
        }

        public static Steering operator *(Steering a, float b)
        {
            return new Steering { steering = a.steering * b };
        }

        public static Steering operator /(Steering a, float b)
        {
            return new Steering { steering = a.steering / b };
        }

        public override string ToString()
        {
            return steering.ToString();
        }
    }

    //public class SteeringClass
    //{
    //    private static Random random = new Random();

    //    //public float maxSpeed = 1;
    //    public float maxForce = 5.4f;
    //    public float maxVelocity = 2.5f;
    //    public float drag = 0.99f;

    //    //public Vector2 direction = Vector2.Zero;
    //    public Vector2 velocity = Vector2.Zero;
    //    public Vector2 desiredVelocity = Vector2.Zero;
    //    public Vector2 steering = Vector2.Zero;
    //    public Vector2 lastSteering = Vector2.Zero;
    //    //public Vector2 daesiredVelocity = Vector2.Zero;

    //    private float _wanderAngle = 0f;

    //    public Vector2 truncate(Vector2 p, float len)
    //    {
    //        var i = len / p.Length;
    //        i = i < 1.0f ? i : 1.0f;

    //        return p * i;
    //    }

    //    //public void face(Vector2 from, Vector2 target, float mass)
    //    //{
    //    //    //don't want to modify the real steering
    //    //    var steering = (target - from).ToNormalized() * maxVelocity;
    //    //    steering = truncate(steering, maxForce);
    //    //    steering /= mass;


    //    //    direction = truncate(velocity + steering, maxVelocity);
    //    //}

    //    public void seek(Vector2 from, Vector2 target)
    //    {
    //        seek(from, target, 1f);
    //    }

    //    public void seek(Vector2 from, Vector2 target, float influence)
    //    {
    //        steering += (target - from).ToNormalized() * maxVelocity * influence;
    //    }

    //    public void arrive(Vector2 from, Vector2 target, float radius, float stopModifier)
    //    {
    //        arrive(from, target, radius, stopModifier, 1f);
    //    }

    //    public void arrive(Vector2 from, Vector2 target, float radius, float stopModifier, float influence)
    //    {
    //        var direction = (target - from).ToNormalized();
    //        desiredVelocity = direction * maxVelocity;

    //        var distance = (target - from).Length;

    //        if (distance < radius)
    //            desiredVelocity *= (distance / radius) / stopModifier;

    //        steering += (desiredVelocity - velocity) * influence;
    //    }

    //    public void flee(Vector2 from, Vector2 target)
    //    {
    //        flee(from, target, 1f);
    //    }

    //    public void flee(Vector2 from, Vector2 target, float influence)
    //    {
    //        var direction = (from - target).ToNormalized();
    //        desiredVelocity = direction * maxVelocity;

    //        steering += (desiredVelocity - velocity) * influence;
    //    }

    //    public void wander(Vector2 from, float distance, float radius, float angleChange)
    //    {
    //        wander(from, distance, radius, angleChange, 1f);
    //    }

    //    public void wander(Vector2 from, float distance, float radius, float angleChange, float influence)
    //    {
    //        var circleCenter = velocity.ToNormalized() * distance;
    //        if (circleCenter == Vector2.Zero)
    //        {
    //            _wanderAngle = (float)random.NextDouble() * Utility.TwoPi;
    //            circleCenter = new Vector2(Utility.Cos(_wanderAngle), Utility.Sin(_wanderAngle)) * distance;
    //        }

    //        var displacement = new Vector2(Utility.Cos(_wanderAngle), Utility.Sin(_wanderAngle)) * radius;
    //        _wanderAngle += (float)random.NextDouble() * angleChange - angleChange * 0.5f;

    //        displacement = truncate(circleCenter + displacement, maxForce);
    //        seek(from, from + displacement, influence);
    //    }

    //    public void pursue(Vector2 from, Vector2 target, Vector2 targetVelocity)
    //    {
    //        pursue(from, target, targetVelocity, 1f);
    //    }

    //    public void pursue(Vector2 from, Vector2 target, Vector2 targetVelocity, float influence)
    //    {
    //        var distance = (from - target);
    //        var t = distance.Length / maxVelocity;
    //        seek(from, target + t * targetVelocity, influence);
    //    }

    //    public void evade(Vector2 from, Vector2 target, Vector2 targetVelocity)
    //    {
    //        evade(from, target, targetVelocity, 1f);
    //    }

    //    public void evade(Vector2 from, Vector2 target, Vector2 targetVelocity, float influence)
    //    {
    //        var distance = (from - target);
    //        var t = distance.Length / maxVelocity;
    //        flee(from, target + t * targetVelocity, influence);
    //    }



    //    public Vector2 integrate(Vector2 p, float mass)
    //    {
    //        if (steering == Vector2.Zero)
    //        {
    //            velocity *= drag;
    //            if (Math.Abs(velocity.X) < 0.1f)
    //                velocity.X = 0f;

    //            if (Math.Abs(velocity.Y) < 0.1f)
    //                velocity.Y = 0f;
    //        }
    //        else
    //        {
    //            steering = truncate(steering, maxForce);
    //            steering /= mass;
    //        }

    //        velocity = truncate(velocity + steering, maxVelocity);

    //        //if (steering != Vector2.Zero)
    //        //    direction = velocity;

    //        lastSteering = steering;
    //        steering = Vector2.Zero;

    //        return p + velocity;
    //    }
    //}
}
