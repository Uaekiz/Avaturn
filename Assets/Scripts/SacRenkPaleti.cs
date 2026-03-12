using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // List kullanabilmek için bunu ekledik

public class SacRenkPaleti : MonoBehaviour
{
    [Header("Renk Seçenekleri")]
    public Color[] sacRenkleri;

    [Header("UI Ayarları")]
    public Transform renkKutusuParent;  
    public GameObject renkButonuPrefab; 

    [Header("Karakter Referansı")]
    public Transform karakterAnaObje;   

    // Sahnede oluşan bütün çerçeveleri hafızada tutacağımız liste
    private List<Outline> tumCerceveler = new List<Outline>();

    void Start()
    {
        ButonlariOlustur();
        
        // Oyun açıldığında daha önce seçilmiş rengi yükle
        string kayitliRenkHex = PlayerPrefs.GetString("SeciliSacRengi", "#FFFFFF");
        Color kayitliRenk;
        if (ColorUtility.TryParseHtmlString(kayitliRenkHex, out kayitliRenk))
        {
            RengiUygula(kayitliRenk);

            // Oyun ilk açıldığında, o an seçili olan rengin çerçevesini otomatik yak
            for (int i = 0; i < sacRenkleri.Length; i++)
            {
                // Eğer dizideki renk, hafızadaki renkle aynıysa o butonu parlat
                if (sacRenkleri[i] == kayitliRenk && tumCerceveler.Count > i)
                {
                    tumCerceveler[i].enabled = true;
                }
            }
        }
    }

    void ButonlariOlustur()
    {
        foreach (Color renk in sacRenkleri)
        {
            GameObject yeniButon = Instantiate(renkButonuPrefab, renkKutusuParent);
            yeniButon.GetComponent<Image>().color = renk; 
            
            // Butonun içindeki Outline (Çerçeve) bileşenini bul ve listeye kaydet
            Outline cerceve = yeniButon.GetComponent<Outline>();
            if (cerceve != null)
            {
                cerceve.enabled = false; // Başlangıçta gizli
                tumCerceveler.Add(cerceve);
            }
            
            // Butona tıklandığında hem rengi hem de KENDİ çerçevesini fonksiyona yollasın
            yeniButon.GetComponent<Button>().onClick.AddListener(() => RenkSec(renk, cerceve));
        }
    }

    // Parametreye 'Outline' eklendi
    public void RenkSec(Color secilenRenk, Outline secilenCerceve)
    {
        // 1. Önce sahnedeki BÜTÜN çerçeveleri kapat (Söndür)
        foreach (Outline c in tumCerceveler)
        {
            if (c != null) c.enabled = false;
        }

        // 2. Sadece şu an tıklanan butonun çerçevesini aç (Yak)
        if (secilenCerceve != null) 
        {
            secilenCerceve.enabled = true;
        }

        // 3. Rengi saça uygula ve hafızaya kaydet
        RengiUygula(secilenRenk);

        string hexRenk = "#" + ColorUtility.ToHtmlStringRGBA(secilenRenk);
        PlayerPrefs.SetString("SeciliSacRengi", hexRenk);
        PlayerPrefs.Save();
    }

    public void RengiUygula(Color renk)
    {
        // O kusursuz çalışan Tag (Etiket) ve Kalkan kırma sistemimiz
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