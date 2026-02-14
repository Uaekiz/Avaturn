using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class KombinSistemi : MonoBehaviour
{
    [Header("Cinsiyet Ayarý")]
    public bool isMale = true;

    [Header("Hafýza")]
    public int seciliSacIndex = 0; // 0 = Kel
    public int seciliKombinIndex = 0;

    [Header("UI Konteynýrlarý")]
    public Transform sacButonParent;    // sac_icerik buraya gelecek
    public Transform kombinButonParent; // kombin_icerik buraya gelecek
    public GameObject butonPrefab;

    [Header("Kombin Listeleri")]
    public List<GameObject> erkekKombinler;
    public List<GameObject> kadinKombinler;

    // Head altýndaki saçlarýn isimleri (Hiyerarþidekiyle ayný sýrada olmalý)
    private string[] sacIsimleri = { "husosac", "italyansac", "serserisac", "uzunsac" };

    void Start()
    {
        ButonlariOlustur();
        KarakteriGuncelle();
    }

    void ButonlariOlustur()
    {
        // 1. SAÇ BUTONLARI
        // Önce Kel butonu
        ButonYarat("Saçsýz (Kel)", 0, true, sacButonParent);
        // Diðer saçlar
        for (int i = 0; i < sacIsimleri.Length; i++)
        {
            ButonYarat(sacIsimleri[i], i + 1, true, sacButonParent);
        }

        // 2. KOMBÝN BUTONLARI
        List<GameObject> aktifListe = isMale ? erkekKombinler : kadinKombinler;
        for (int i = 0; i < aktifListe.Count; i++)
        {
            ButonYarat(aktifListe[i].name, i, false, kombinButonParent);
        }
    }

    void ButonYarat(string ad, int index, bool sacMi, Transform parent)
    {
        GameObject yeniButon = Instantiate(butonPrefab, parent);
        yeniButon.GetComponentInChildren<TextMeshProUGUI>().text = ad;
        yeniButon.GetComponent<Button>().onClick.AddListener(() => {
            if (sacMi) SacDegistir(index); else KombinDegistir(index);
        });
    }

    public void SacDegistir(int index) { seciliSacIndex = index; SaciUygula(); }
    public void KombinDegistir(int index) { seciliKombinIndex = index; KarakteriGuncelle(); }

    void KarakteriGuncelle()
    {
        List<GameObject> liste = isMale ? erkekKombinler : kadinKombinler;
        // Hepsini kapat
        foreach (var k in erkekKombinler) if (k) k.SetActive(false);
        foreach (var k in kadinKombinler) if (k) k.SetActive(false);

        // Seçiliyi aç
        if (seciliKombinIndex < liste.Count)
        {
            liste[seciliKombinIndex].SetActive(true);
            SaciUygula();
        }
    }

    void SaciUygula()
    {
        List<GameObject> liste = isMale ? erkekKombinler : kadinKombinler;
        GameObject aktifKombin = liste[seciliKombinIndex];

        // Senin hiyerarþindeki tam yol: Armature/Hips/Spine/Spine1/Spine2/Neck/Head
        Transform head = aktifKombin.transform.Find("Armature/Hips/Spine/Spine1/Spine2/Neck/Head");

        if (head != null)
        {
            // Tüm saçlarý kapat
            for (int i = 0; i < head.childCount; i++) head.GetChild(i).gameObject.SetActive(false);

            // Seçili saçý aç (0 deðilse)
            if (seciliSacIndex > 0)
            {
                int childIndex = seciliSacIndex - 1;
                if (childIndex < head.childCount) head.GetChild(childIndex).gameObject.SetActive(true);
            }
        }
    }
}