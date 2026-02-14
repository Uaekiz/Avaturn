using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class KombinSistemi : MonoBehaviour
{
    [Header("Cinsiyet Ayarı")]
    public bool isMale = true;

    [Header("Hafıza")]
    public int seciliSacIndex = 3;
    public int seciliKombinIndex = 0;

    [Header("UI Konteynırları")]
    public Transform sacButonParent;    
    public Transform kombinButonParent; 
    public GameObject butonPrefab;

    [Header("Kombin Listeleri")]
    public List<GameObject> erkekKombinler;
    public List<GameObject> kadinKombinler;

    [Header("Buton Renk Ayarları")]
    public Color seciliRenk = new Color(0.9f, 0.9f, 0.9f, 1f);
    public Color normalRenk = new Color(0.4f, 0.4f, 0.4f, 1f); 

    [Header("Hiyerarşi Ayarı")]
    public string kafaYolu = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head"; // Eğer kadın modelinde farklıysa buradan değiştirirsin

// Eski sacIsimleri dizisini sildik, artik isimleri direkt modelden okuyacağız.
    // Buton referanslarını tutan listeler
    private List<Button> yaratilanSacButonlari = new List<Button>();
    private List<Button> yaratilanKombinButonlari = new List<Button>();

    void Start()
    {
        ButonlariOlustur();
        KarakteriGuncelle();
        ButonRenkleriniGuncelle();
    }

    void ButonlariOlustur()
{
    // Eski butonları temizle (Fiziksel silme)
    foreach (Transform child in sacButonParent) Destroy(child.gameObject);
    foreach (Transform child in kombinButonParent) Destroy(child.gameObject);

    yaratilanSacButonlari.Clear();
    yaratilanKombinButonlari.Clear();

    List<GameObject> hedefKombinler = isMale ? erkekKombinler : kadinKombinler;
    if (hedefKombinler.Count == 0) return;

    // 1. DİNAMİK SAÇ BUTONLARI
    // İlk kıyafetin kafasını bulup içindeki saçları listeliyoruz
    Transform head = hedefKombinler[0].transform.Find(kafaYolu);
    
    ButonYarat("Kel", 0, true, sacButonParent); // Her zaman en üstte kel butonu
    
    if (head != null)
    {
        for (int i = 0; i < head.childCount; i++)
        {
            // Modelin içindeki objenin adını direkt buton yazısı yapıyoruz
            string sacObjesininAdi = head.GetChild(i).name;
            ButonYarat(sacObjesininAdi, i + 1, true, sacButonParent);
        }
    }

    // 2. KOMBİN BUTONLARI
    for (int i = 0; i < hedefKombinler.Count; i++)
    {
        ButonYarat(hedefKombinler[i].name, i, false, kombinButonParent);
    }
}

    void ButonYarat(string isim, int index, bool isSac, Transform parent)
    {
        GameObject yeniButon = Instantiate(butonPrefab, parent);
        
        TextMeshProUGUI yazi = yeniButon.GetComponentInChildren<TextMeshProUGUI>();
        if (yazi) yazi.text = isim;

        Button btn = yeniButon.GetComponent<Button>();
        if (btn)
        {
            if (isSac) yaratilanSacButonlari.Add(btn);
            else yaratilanKombinButonlari.Add(btn);

            btn.onClick.AddListener(() => 
            {
                if (isSac) SacDegistir(index);
                else KombinDegistir(index);
            });
        }
    }

    public void SacDegistir(int index) 
    { 
        seciliSacIndex = index; 
        SaciUygula(); 
        ButonRenkleriniGuncelle(); 
    }

    public void KombinDegistir(int index) 
    { 
        seciliKombinIndex = index; 
        KarakteriGuncelle(); 
        ButonRenkleriniGuncelle(); 
    }

    void ButonRenkleriniGuncelle()
    {
        // 1. Saç Butonlarını Boya
        for (int i = 0; i < yaratilanSacButonlari.Count; i++)
        {
            Image img = yaratilanSacButonlari[i].GetComponent<Image>();
            if (img)
            {
                img.color = (i == seciliSacIndex) ? seciliRenk : normalRenk;
            }
        }

        // 2. Kombin Butonlarını Boya
        for (int i = 0; i < yaratilanKombinButonlari.Count; i++)
        {
            Image img = yaratilanKombinButonlari[i].GetComponent<Image>();
            if (img)
            {
                img.color = (i == seciliKombinIndex) ? seciliRenk : normalRenk;
            }
        }
    }

    void KarakteriGuncelle()
    {
        List<GameObject> liste = isMale ? erkekKombinler : kadinKombinler;
        
        foreach (var k in erkekKombinler) if (k) k.SetActive(false);
        foreach (var k in kadinKombinler) if (k) k.SetActive(false);

        if (seciliKombinIndex < liste.Count && liste[seciliKombinIndex] != null)
        {
            liste[seciliKombinIndex].SetActive(true);
            SaciUygula();
        }
    }

    void SaciUygula()
{
    List<GameObject> liste = isMale ? erkekKombinler : kadinKombinler;
    if (seciliKombinIndex >= liste.Count) return;

    GameObject aktifKombin = liste[seciliKombinIndex];
    Transform head = aktifKombin.transform.Find(kafaYolu);

    if (head != null)
    {
        // Önce tüm saçları kapat
        foreach (Transform child in head) child.gameObject.SetActive(false);

        // Eğer "Kel" (0) seçili değilse, ilgili indeksteki saçı aç
        if (seciliSacIndex > 0)
        {
            int childIndex = seciliSacIndex - 1;
            if (childIndex < head.childCount)
            {
                head.GetChild(childIndex).gameObject.SetActive(true);
            }
        }
    }
}
}