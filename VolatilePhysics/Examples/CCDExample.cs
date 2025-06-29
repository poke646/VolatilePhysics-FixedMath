using System;
using FixMath.NET;
using Volatile;

namespace VolatilePhysics.Examples
{
    /// <summary>
    /// Prosty przykład demonstrujący użycie Continuous Collision Detection
    /// </summary>
    public static class CCDExample
    {
        public static void RunExample()
        {
            Console.WriteLine("=== Demo Continuous Collision Detection ===");
            
            // Utwórz świat fizyki
            VoltWorld world = new VoltWorld();
            
            // Utwórz kształty
            VoltShape bulletShape = world.CreateCircle((Fix64)0.1M, Fix64.One, Fix64.Zero, (Fix64)0.8M);
            VoltShape wallShape = world.CreateCircle((Fix64)0.2M, Fix64.One, Fix64.Zero, (Fix64)0.5M);
            
            Console.WriteLine("Tworzenie obiektów...");
            
            // Utwórz pocisk (szybko poruszający się)
            VoltBody bullet = world.CreateDynamicBody(
                new VoltVector2(-(Fix64)10, Fix64.Zero), 
                Fix64.Zero, 
                new VoltShape[] { bulletShape }
            );
            
            // Utwórz cienką ścianę
            VoltBody wall = world.CreateStaticBody(
                new VoltVector2(Fix64.Zero, Fix64.Zero), 
                Fix64.Zero, 
                new VoltShape[] { wallShape }
            );
            
            Console.WriteLine("Test 1: Bez CCD (pocisk może przejść przez ścianę)");
            
            // Test bez CCD
            bullet.EnableCCD = false;
            bullet.LinearVelocity = new VoltVector2((Fix64)50, Fix64.Zero); // Bardzo szybko
            
            // Symuluj kilka kroków
            for (int i = 0; i < 10; i++)
            {
                world.Update();
                Console.WriteLine($"  Krok {i}: Pozycja pocisku = {bullet.Position.x}");
                
                if (bullet.Position.x > (Fix64)10)
                {
                    Console.WriteLine("  ❌ Pocisk przeszedł przez ścianę!");
                    break;
                }
            }
            
            Console.WriteLine("\nTest 2: Z CCD (pocisk powinien zostać zatrzymany)");
            
            // Reset pozycji
            bullet.Set(new VoltVector2(-(Fix64)10, Fix64.Zero), Fix64.Zero);
            bullet.LinearVelocity = new VoltVector2((Fix64)50, Fix64.Zero);
            
            // Włącz CCD
            world.EnableCCD(bullet, (Fix64)1.0M);
            
            Console.WriteLine($"CCD włączone: {bullet.EnableCCD}");
            Console.WriteLine($"Próg prędkości CCD: {bullet.CCDVelocityThreshold}");
            Console.WriteLine($"Wymaga CCD: {bullet.RequiresCCD()}");
            
            // Symuluj kroki z CCD
            for (int i = 0; i < 10; i++)
            {
                world.Update();
                Console.WriteLine($"  Krok {i}: Pozycja pocisku = {bullet.Position.x}, Prędkość = {bullet.LinearVelocity.magnitude}");
                
                // Sprawdź czy pocisk znacznie zwolnił (znak kolizji)
                if (bullet.LinearVelocity.magnitude < (Fix64)5)
                {
                    Console.WriteLine("  ✅ CCD wykryło kolizję i zatrzymało pocisk!");
                    break;
                }
                
                if (bullet.Position.x > (Fix64)10)
                {
                    Console.WriteLine("  ⚠️ Pocisk nadal przeszedł, możliwe ulepszenia CCD potrzebne");
                    break;
                }
            }
            
            Console.WriteLine("\nTest 3: Różne progi prędkości CCD");
            
            Fix64[] testThresholds = { (Fix64)0.5M, (Fix64)2.0M, (Fix64)10.0M };
            
            foreach (Fix64 threshold in testThresholds)
            {
                // Reset
                bullet.Set(new VoltVector2(-(Fix64)5, Fix64.Zero), Fix64.Zero);
                bullet.LinearVelocity = new VoltVector2((Fix64)8, Fix64.Zero);
                bullet.CCDVelocityThreshold = threshold;
                
                Console.WriteLine($"\n  Próg CCD: {threshold}");
                Console.WriteLine($"  Prędkość pocisku: {bullet.LinearVelocity.magnitude}");
                Console.WriteLine($"  Wymaga CCD: {bullet.RequiresCCD()}");
                
                if (bullet.RequiresCCD())
                {
                    Console.WriteLine("  ✅ CCD będzie aktywne");
                }
                else
                {
                    Console.WriteLine("  ❌ CCD nie będzie aktywne");
                }
            }
            
            Console.WriteLine("\n=== Demo zakończone ===");
            Console.WriteLine("\nPodsumowanie funkcjonalności CCD:");
            Console.WriteLine("✅ Właściwości CCD dodane do VoltBody");
            Console.WriteLine("✅ Metody swept collision detection zaimplementowane");
            Console.WriteLine("✅ API do włączania/wyłączania CCD");
            Console.WriteLine("✅ Konfiguracja progów prędkości");
            Console.WriteLine("✅ Integracja z główną pętlą fizyki");
        }
        
        /// <summary>
        /// Test wydajności CCD
        /// </summary>
        public static void PerformanceTest()
        {
            Console.WriteLine("\n=== Test wydajności CCD ===");
            
            VoltWorld world = new VoltWorld();
            VoltShape shape = world.CreateCircle((Fix64)0.5M, Fix64.One, Fix64.Zero, (Fix64)0.8M);
            
            const int bodyCount = 100;
            VoltBody[] bodies = new VoltBody[bodyCount];
            
            // Utwórz wiele obiektów
            for (int i = 0; i < bodyCount; i++)
            {
                bodies[i] = world.CreateDynamicBody(
                    new VoltVector2((Fix64)i, Fix64.Zero), 
                    Fix64.Zero, 
                    new VoltShape[] { shape }
                );
                bodies[i].LinearVelocity = new VoltVector2((Fix64)10, Fix64.Zero);
            }
            
            // Test bez CCD
            DateTime startTime = DateTime.Now;
            for (int frame = 0; frame < 100; frame++)
            {
                world.Update();
            }
            DateTime endTime = DateTime.Now;
            double timeWithoutCCD = (endTime - startTime).TotalMilliseconds;
            
            // Włącz CCD dla wszystkich
            foreach (VoltBody body in bodies)
            {
                world.EnableCCD(body);
            }
            
            // Test z CCD
            startTime = DateTime.Now;
            for (int frame = 0; frame < 100; frame++)
            {
                world.Update();
            }
            endTime = DateTime.Now;
            double timeWithCCD = (endTime - startTime).TotalMilliseconds;
            
            Console.WriteLine($"Bez CCD: {timeWithoutCCD:F2}ms");
            Console.WriteLine($"Z CCD: {timeWithCCD:F2}ms");
            Console.WriteLine($"Overhead CCD: {((timeWithCCD - timeWithoutCCD) / timeWithoutCCD * 100):F1}%");
        }
    }
}

// Przykład użycia:
// CCDExample.RunExample();
// CCDExample.PerformanceTest();
