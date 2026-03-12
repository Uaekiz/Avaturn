using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input Sistemi
using UnityEngine.EventSystems; // UI Tıklamasını algılamak için

public class KarakterDondurme : MonoBehaviour
{
    [Header("Ayarlar")]
    public float donusHizi = 0.2f; // Dönme hassasiyeti
    public bool tersYon = true;    // Parmağı sola çekince karakter sola mı dönsün?

    void Update()
    {
        // 1. Eğer bir UI butonuna (Kıyafet seçimi vs.) basılıyorsa dönme!
        if (IsPointerOverUI()) return;

        // 2. DOKUNMATİK (Android/iOS)
        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
        {
            var dokunma = Touchscreen.current.touches[0];

            // Sadece parmak hareket halindeyse (Moved) döndür
            if (dokunma.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                Vector2 delta = dokunma.delta.ReadValue();
                Dondur(delta.x);
            }
        }
        // 3. MOUSE (PC Testi İçin - Sol Tık Basılıyken)
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
             Vector2 mouseDelta = Mouse.current.delta.ReadValue();
             Dondur(mouseDelta.x);
        }
    }

    void Dondur(float miktar)
    {
        // Çok küçük titremeleri (parmak titremesi) yok say
        if (Mathf.Abs(miktar) < 0.1f) return;

        float yonCarpan = tersYon ? -1 : 1;
        
        // Y ekseninde (kendi etrafında) döndür
        // Platformun kendisini döndürdüğümüz için içindeki karakterler de döner.
        transform.Rotate(0, miktar * donusHizi * yonCarpan, 0);
    }

    // UI'a tıklanıp tıklanmadığını kontrol eden güvenlik fonksiyonu
    private bool IsPointerOverUI()
    {
        // 1. Standart UI kontrolü
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        
        // 2. Mobilde bazen üstteki yetmez, parmak ID'si ile kontrol gerekir
        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
        {
            int touchId = Touchscreen.current.touches[0].touchId.ReadValue();
            if (EventSystem.current.IsPointerOverGameObject(touchId)) return true;
        }

        return false;
    }
}