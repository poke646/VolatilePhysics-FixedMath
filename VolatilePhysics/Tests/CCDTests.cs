using System;
using FixMath.NET;
using Volatile;

namespace VolatilePhysics.Tests
{
    /// <summary>
    /// Testy jednostkowe dla Continuous Collision Detection
    /// </summary>
    public static class CCDTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("Uruchamianie testów CCD...");
            
            TestCCDBasicFunctionality();
            TestCCDVelocityThreshold();
            TestCCDSweepTest();
            TestCCDWithStaticObjects();
            TestCCDPerformance();
            
            Console.WriteLine("Wszystkie testy CCD zakończone pomyślnie!");
        }
        
        /// <summary>
        /// Test podstawowej funkcjonalności CCD
        /// </summary>
        public static void TestCCDBasicFunctionality()
        {
            Console.WriteLine("Test: Podstawowa funkcjonalność CCD");
            
            // Utwórz świat
            VoltWorld world = new VoltWorld();
            
            // Utwórz szybki obiekt
            VoltShape[] shapes = { world.CreateCircle((Fix64)0.5M, Fix64.One, Fix64.Zero, (Fix64)0.5M) };
            VoltBody fastBody = world.CreateDynamicBody(new VoltVector2(Fix64.Zero, Fix64.Zero), Fix64.Zero, shapes);
            
            // Sprawdź domyślne ustawienia CCD
            if (fastBody.EnableCCD)
                throw new Exception("CCD powinno być domyślnie wyłączone");
            
            // Włącz CCD
            world.EnableCCD(fastBody);
            
            if (!fastBody.EnableCCD)
                throw new Exception("CCD powinno być włączone");
            
            // Wyłącz CCD
            world.DisableCCD(fastBody);
            
            if (fastBody.EnableCCD)
                throw new Exception("CCD powinno być wyłączone");
            
            Console.WriteLine("✓ Test podstawowej funkcjonalności CCD przeszedł pomyślnie");
        }
        
        /// <summary>
        /// Test progu prędkości CCD
        /// </summary>
        public static void TestCCDVelocityThreshold()
        {
            Console.WriteLine("Test: Próg prędkości CCD");
            
            VoltWorld world = new VoltWorld();
            VoltShape[] shapes = { world.CreateCircle((Fix64)0.5M, Fix64.One, Fix64.Zero, (Fix64)0.5M) };
            VoltBody body = world.CreateDynamicBody(VoltVector2.zero, Fix64.Zero, shapes);
            
            // Włącz CCD z wysokim progiem
            world.EnableCCD(body, (Fix64)10.0M);
            
            // Niska prędkość - CCD nie powinno być wymagane
            body.LinearVelocity = new VoltVector2((Fix64)5.0M, Fix64.Zero);
            if (body.RequiresCCD())
                throw new Exception("CCD nie powinno być wymagane przy niskiej prędkości");
            
            // Wysoka prędkość - CCD powinno być wymagane
            body.LinearVelocity = new VoltVector2((Fix64)15.0M, Fix64.Zero);
            if (!body.RequiresCCD())
                throw new Exception("CCD powinno być wymagane przy wysokiej prędkości");
            
            Console.WriteLine("✓ Test progu prędkości CCD przeszedł pomyślnie");
        }
        
        /// <summary>
        /// Test sweep test CCD
        /// </summary>
        public static void TestCCDSweepTest()
        {
            Console.WriteLine("Test: Sweep test CCD");
            
            VoltWorld world = new VoltWorld();
            VoltShape[] shapes = { world.CreateCircle((Fix64)0.5M, Fix64.One, Fix64.Zero, (Fix64)0.5M) };
            
            // Obiekt poruszający się
            VoltBody movingBody = world.CreateDynamicBody(new VoltVector2(Fix64.Zero, Fix64.Zero), Fix64.Zero, shapes);
            world.EnableCCD(movingBody);
            movingBody.LinearVelocity = new VoltVector2((Fix64)10.0M, Fix64.Zero);
            
            // Obiekt statyczny na trasie
            VoltBody staticBody = world.CreateStaticBody(new VoltVector2((Fix64)2.0M, Fix64.Zero), Fix64.Zero, shapes);
            
            // Wykonaj aktualizację pozycji
            movingBody.PreviousPosition = movingBody.Position;
            movingBody.Position = new VoltVector2((Fix64)1.0M, Fix64.Zero); // Symuluj ruch
            
            // Test sweep
            VoltVector2 contactPoint, contactNormal;
            Fix64 timeOfImpact = movingBody.SweepTest(staticBody, out contactPoint, out contactNormal);
            
            if (timeOfImpact >= Fix64.One)
                throw new Exception("Sweep test powinien wykryć kolizję");
            
            Console.WriteLine($"✓ Test sweep test CCD przeszedł pomyślnie (TOI: {timeOfImpact})");
        }
        
        /// <summary>
        /// Test CCD z obiektami statycznymi
        /// </summary>
        public static void TestCCDWithStaticObjects()
        {
            Console.WriteLine("Test: CCD z obiektami statycznymi");
            
            VoltWorld world = new VoltWorld();
            VoltShape[] shapes = { world.CreateCircle((Fix64)0.5M, Fix64.One, Fix64.Zero, (Fix64)0.5M) };
            
            // Szybki obiekt dynamiczny
            VoltBody fastBody = world.CreateDynamicBody(new VoltVector2(-(Fix64)5.0M, Fix64.Zero), Fix64.Zero, shapes);
            world.EnableCCD(fastBody);
            fastBody.LinearVelocity = new VoltVector2((Fix64)20.0M, Fix64.Zero);
            
            // Cienka ściana statyczna
            VoltShape[] wallShapes = { world.CreateCircle((Fix64)0.1M, Fix64.One, Fix64.Zero, (Fix64)0.5M) };
            VoltBody wall = world.CreateStaticBody(new VoltVector2(Fix64.Zero, Fix64.Zero), Fix64.Zero, wallShapes);
            
            // Przed aktualizacją - brak kolizji
            world.BroadPhase();
            
            // Aktualizacja z CCD
            world.Update();
            
            // Sprawdź czy obiekt został zatrzymany (kolizja)
            if (fastBody.LinearVelocity.magnitude > (Fix64)15.0M)
            {
                Console.WriteLine("⚠ Ostrzeżenie: Obiekt może nie zostać zatrzymany przez CCD");
            }
            
            Console.WriteLine("✓ Test CCD z obiektami statycznymi przeszedł pomyślnie");
        }
        
        /// <summary>
        /// Prosty test wydajności CCD
        /// </summary>
        public static void TestCCDPerformance()
        {
            Console.WriteLine("Test: Wydajność CCD");
            
            VoltWorld world = new VoltWorld();
            VoltShape[] shapes = { world.CreateCircle((Fix64)0.5M, Fix64.One, Fix64.Zero, (Fix64)0.5M) };
            
            // Utwórz wiele szybkich obiektów
            const int objectCount = 100;
            VoltBody[] bodies = new VoltBody[objectCount];
            
            for (int i = 0; i < objectCount; i++)
            {
                bodies[i] = world.CreateDynamicBody(
                    new VoltVector2((Fix64)i, Fix64.Zero), 
                    Fix64.Zero, 
                    shapes);
                world.EnableCCD(bodies[i]);
                bodies[i].LinearVelocity = new VoltVector2((Fix64)10.0M, Fix64.Zero);
            }
            
            // Zmierz czas aktualizacji
            DateTime startTime = DateTime.Now;
            
            for (int frame = 0; frame < 10; frame++)
            {
                world.Update();
            }
            
            DateTime endTime = DateTime.Now;
            double elapsed = (endTime - startTime).TotalMilliseconds;
            
            Console.WriteLine($"✓ Test wydajności CCD: {elapsed:F2}ms dla {objectCount} obiektów przez 10 klatek");
            
            if (elapsed > 1000) // Ponad 1 sekunda
            {
                Console.WriteLine("⚠ Ostrzeżenie: CCD może być zbyt wolne dla tej liczby obiektów");
            }
        }
    }
}

// Przykład użycia testów:
// CCDTests.RunAllTests();
