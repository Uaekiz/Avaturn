using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SacRenkPaleti : MonoBehaviour
{
    [Header("Renk Seçenekleri")]
    public Color[] sacRenkleri;

    [Header("UI Ayarları")]
    public Transform renkKutusuParent;  
    public GameObject renkButonuPrefab; 

    [Header("Karakter Referansı")]
    public Transform karakterAnaObje;   

    // Artık çerçeve resimlerini tutacağız
    private List<Image> tumCerceveler = new List<Image>();

    void Start()
    {
        ButonlariOlustur();
        
        string kayitliRenkHex = PlayerPrefs.GetString("SeciliSacRengi", "#FFFFFF");
        Color kayitliRenk;
        if (ColorUtility.TryParseHtmlString(kayitliRenkHex, out kayitliRenk))
        {
            RengiUygula(kayitliRenk);

            // Oyun ilk açıldığında, o an seçili olan rengin çerçevesini yak
            for (int i = 0; i < sacRenkleri.Length; i++)
            {
                if (sacRenkleri[i] == kayitliRenk && tumCerceveler.Count > i)
                {
                    tumCerceveler[i].color = Color.white; // Seçili olanı bembeyaz yap
                }
            }
        }
    }

    void ButonlariOlustur()
    {
        foreach (Color renk in sacRenkleri)
        {
            // 1. ANA BUTON = ARKA PLAN / ÇERÇEVE
            GameObject cerceveButonu = Instantiate(renkButonuPrefab, renkKutusuParent);
            Image cerceveResmi = cerceveButonu.GetComponent<Image>();
            
            // Çerçeveyi başlangıçta görünmez (saydam) yapıyoruz
            cerceveResmi.color = new Color(1f, 1f, 1f, 0f); 
            tumCerceveler.Add(cerceveResmi);

            // 2. BUTONUN İÇİNDEKİ KUTU = ASIL RENK 
            GameObject renkKutusu = new GameObject("Asil_Renk");
            renkKutusu.transform.SetParent(cerceveButonu.transform, false);

            Image renkResmi = renkKutusu.AddComponent<Image>();
            renkResmi.color = renk; // Senin seçtiğin rengi buna veriyoruz
            
            // Tıklamayı engellemesin diye bunu kapatıyoruz (Tıklamayı ana buton algılayacak)
            renkResmi.raycastTarget = false; 

            // Renk kutusunu çerçevenin içinden "8 piksel" daraltıyoruz!
            RectTransform rt = renkKutusu.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(8f, 8f);   // Sol ve Alttan 8 piksel boşluk bırak
            rt.offsetMax = new Vector2(-8f, -8f); // Sağ ve Üstten 8 piksel boşluk bırak

            // 3. Tıklama olayı
            cerceveButonu.GetComponent<Button>().onClick.AddListener(() => RenkSec(renk, cerceveResmi));
        }
    }

    public void RenkSec(Color secilenRenk, Image secilenCerceve)
    {
        // 1. Önce bütün çerçeveleri söndür (Saydam yap)
        foreach (Image c in tumCerceveler)
        {
            if (c != null) c.color = new Color(1f, 1f, 1f, 0f);
        }

        // 2. Sadece tıklanan butonun çerçevesini aç (Bembeyaz yap)
        if (secilenCerceve != null) 
        {
            secilenCerceve.color = Color.white;
        }

        RengiUygula(secilenRenk);

        string hexRenk = "#" + ColorUtility.ToHtmlStringRGBA(secilenRenk);
        PlayerPrefs.SetString("SeciliSacRengi", hexRenk);
        PlayerPrefs.Save();
    }

    public void RengiUygula(Color renk)
    {
        Renderer[] tumRendererlar = karakterAnaObje.GetComponentsInChildren<Renderer>(true);
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();

        foreach (Renderer ren in tumRendererlar)
        {
            if (ren.gameObject.CompareTag("Hair"))
            {
                ren.GetPropertyBlock(mpb);
                mpb.SetColor("_BaseColor", renk);           
                mpb.SetColor("_Color", renk);               
                mpb.SetColor("baseColorFactor", renk);      
                mpb.SetColor("_BaseColorMap_Color", renk);  
                ren.SetPropertyBlock(mpb);
            }
        }
    }
}