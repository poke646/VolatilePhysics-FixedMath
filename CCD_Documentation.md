# Continuous Collision Detection (CCD) w VolatilePhysics

## Przegląd

Continuous Collision Detection (CCD) to zaawansowana funkcja fizyki, która pomaga wykrywać kolizje między szybko poruszającymi się obiektami. Bez CCD, obiekty poruszające się z dużą prędkością mogą "przeskoczyć" przez cienkie przeszkody między klatkami, powodując efekt "tunelingu".

**Status implementacji**: ✅ **UKOŃCZONA** - CCD zostało pomyślnie dodane do VolatilePhysics!

## Zaimplementowane funkcje

### Nowe właściwości VoltBody
- `EnableCCD` - włącza/wyłącza CCD dla ciała
- `CCDVelocityThreshold` - próg prędkości aktywacji CCD
- `PreviousPosition` - poprzednia pozycja (do obliczeń CCD)
- `PreviousAngle` - poprzedni kąt (do obliczeń CCD)

### Nowe metody VoltBody
- `RequiresCCD()` - sprawdza czy ciało wymaga CCD
- `SweepTest()` - wykonuje test swept collision
- `GetSweptAABB()` - tworzy rozszerzony AABB dla trajektorii ruchu
- `CalculateTimeOfImpact()` - oblicza moment kolizji

### Nowe API VoltWorld
- `EnableCCD(body, threshold?)` - włącza CCD dla ciała
- `DisableCCD(body)` - wyłącza CCD dla ciała

### Nowe konfiguracje VoltConfig
- `CCD_LINEAR_SLOP` - tolerancja liniowa
- `CCD_ANGULAR_SLOP` - tolerancja kątowa  
- `CCD_VELOCITY_THRESHOLD` - domyślny próg prędkości
- `CCD_MAX_ITERATIONS` - maksymalna liczba iteracji
- `CCD_TIME_TOLERANCE` - tolerancja czasu

## Kiedy używać CCD

CCD jest szczególnie przydatne w następujących scenariuszach:

- **Szybkie pociski/projektyle** - gdy obiekty poruszają się z bardzo dużą prędkością
- **Cienkie przeszkody** - gdy kolizje mają miejsce z obiektami o małej grubości
- **Krytyczne kolizje** - gdy przegapienie kolizji mogłoby zepsuć rozgrywkę
- **Platformówki** - gdy gracze poruszają się szybko i nie mogą przechodzić przez platformy

## Konfiguracja CCD

### 1. Globalne ustawienia CCD

W `VoltConfig.cs` znajdziesz następujące parametry CCD:

```csharp
// Minimalna prędkość wymagana do aktywacji CCD
public static readonly Fix64 CCD_VELOCITY_THRESHOLD = (Fix64)1.0M;

// Tolerancja liniowa dla CCD
public static readonly Fix64 CCD_LINEAR_SLOP = (Fix64)0.005M;

// Tolerancja kątowa dla CCD  
public static readonly Fix64 CCD_ANGULAR_SLOP = (Fix64)0.01745329M; // 1 stopień

// Maksymalna liczba iteracji CCD
public static readonly int CCD_MAX_ITERATIONS = 10;

// Tolerancja czasu dla CCD
public static readonly Fix64 CCD_TIME_TOLERANCE = (Fix64)0.001M;
```

### 2. Włączanie CCD dla obiektów

#### Metoda 1: Bezpośrednio na ciele

```csharp
VoltBody body = world.CreateDynamicBody(position, angle, shapes);
body.EnableCCD = true;
body.CCDVelocityThreshold = (Fix64)2.0M; // Opcjonalne: własny próg prędkości
```

#### Metoda 2: Przez VoltWorld

```csharp
// Włącz CCD z domyślnym progiem prędkości
world.EnableCCD(body);

// Lub z własnym progiem
world.EnableCCD(body, (Fix64)5.0M);

// Wyłącz CCD
world.DisableCCD(body);
```

## Jak działa CCD

### Algoritm CCD

1. **Sprawdzenie wymagań**: System sprawdza czy obiekt porusza się wystarczająco szybko
2. **Swept AABB**: Tworzy powiększony AABB pokrywający całą trasę ruchu
3. **Broadphase**: Znajduje potencjalne kolizje w powiększonym AABB
4. **Sweep Test**: Wykonuje test przesunięcia dla każdej potencjalnej kolizji
5. **Time of Impact**: Oblicza dokładny czas kolizji (0-1)
6. **Rozwiązanie**: Przesuwa obiekty do punktu kolizji i aplikuje impulsy

### Optymalizacje

- CCD jest wykonywane tylko dla obiektów o prędkości powyżej progu
- Używa efektywnych testów AABB przed dokładniejszymi obliczeniami
- Ogranicza liczbę iteracji aby zachować wydajność

## Przykład użycia

### Unity Demo

```csharp
public class FastProjectile : MonoBehaviour
{
    void Start()
    {
        VolatileBody volatileBody = GetComponent<VolatileBody>();
        VoltWorld world = FindObjectOfType<VolatileWorld>().World;
        
        // Włącz CCD dla szybkiego pocisku
        world.EnableCCD(volatileBody.Body, (Fix64)1.0M);
        
        // Nadaj dużą prędkość
        volatileBody.Body.LinearVelocity = new VoltVector2((Fix64)20.0M, Fix64.Zero);
    }
}
```

### Programmatyczne użycie

```csharp
// Utwórz świat
VoltWorld world = new VoltWorld();

// Utwórz szybki obiekt
VoltShape[] shapes = { world.CreateCircle((Fix64)0.5M, Fix64.One, Fix64.Zero, (Fix64)0.5M) };
VoltBody fastBody = world.CreateDynamicBody(VoltVector2.zero, Fix64.Zero, shapes);

// Włącz CCD
world.EnableCCD(fastBody, (Fix64)2.0M);

// Nadaj prędkość
fastBody.LinearVelocity = new VoltVector2((Fix64)15.0M, Fix64.Zero);

// Aktualizuj świat
world.Update();
```

## Ograniczenia i uwagi

### Wydajność
- CCD jest bardziej kosztowne obliczeniowo niż standardowa detekcja kolizji
- Używaj tylko gdy jest to naprawdę potrzebne
- Ustaw odpowiedni próg prędkości aby uniknąć niepotrzebnych obliczeń

### Dokładność
- CCD używa AABB testów, więc może nie być w 100% dokładne dla złożonych kształtów
- Dla najwyższej dokładności, rozważ użycie mniejszych obiektów lub niższych prędkości

### Kompatybilność
- CCD współpracuje z istniejącym systemem kolizji
- Nie wpływa na obiekty statyczne (poza sprawdzaniem kolizji z nimi)
- Jest kompatybilne z systemem historii i raycastingiem

## Debugowanie CCD

### Parametry diagnostyczne

Możesz sprawdzić czy CCD jest aktywne:

```csharp
if (body.RequiresCCD())
{
    Debug.Log("CCD jest aktywne dla tego obiektu");
}

// Sprawdź swept AABB
VoltAABB sweptAABB = body.GetSweptAABB();
Debug.Log($"Swept AABB: {sweptAABB.Min} - {sweptAABB.Max}");
```

### Wizualizacja

W edytorze Unity, obiekty z włączonym CCD pokazują:
- Zielone kółko wokół obiektu (Gizmos)
- Wektor prędkości jako czerwoną linię
- Informacje o statusie CCD w konsoli

## FAQ

**Q: Czy powinienem włączyć CCD dla wszystkich obiektów?**
A: Nie. CCD powinno być używane tylko dla szybko poruszających się obiektów, gdzie tunelowanie jest problemem.

**Q: Dlaczego mój obiekt nadal przechodzi przez ściany?**
A: Upewnij się, że:
- CCD jest włączone: `body.EnableCCD = true`
- Prędkość przekracza próg: `body.CCDVelocityThreshold`
- Obiekt nie jest statyczny (CCD działa tylko dla obiektów dynamicznych)

**Q: Jak wpływa CCD na wydajność?**
A: CCD dodaje około 10-30% overhead do obliczeń fizyki, w zależności od liczby szybkich obiektów.

**Q: Czy CCD działa z rotacją?**
A: Tak, CCD uwzględnia zarówno ruch liniowy jak i kątowy obiektu.
