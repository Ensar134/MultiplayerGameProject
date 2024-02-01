using UnityEngine;
using Mirror;

public class Odun : NetworkBehaviour
{
    private Vector3 baslangicPozisyonu;
    private Vector3 baslangicRotasyonu;
    private Rigidbody rb;

    [SyncVar]
    private bool isDropped = false;

    public bool IsDropped
    {
        get { return isDropped; }
    }
    
    void Start()
    {
        var transform1 = transform;
        baslangicPozisyonu = transform1.localPosition;
        baslangicRotasyonu = transform1.localEulerAngles;
        rb = GetComponent<Rigidbody>();
        
    }

    #region OdunDuserme

    [Server]
    
    public void ServerOdunuDusur()
    {
        OdunuDusur();
    }
    
    
    [ClientRpc]
    public void RpcOdunDusur()
    {
        OdunuDusur();
    }
    public void OdunuDusur()
    {
        if (!isDropped)
        {
            rb.isKinematic = false;
            isDropped = true;
        }
    }

    #endregion




    #region ResetOdun
    [Server]
    public void OdunuResetle()
    {
        rb.isKinematic = true; // Odunun fiziksel hareketlerini geçici olarak durdurun
        transform.localPosition = baslangicPozisyonu; // Başlangıç pozisyonuna ayarla
        transform.localEulerAngles = baslangicRotasyonu; // Başlangıç rotasyonuna ayarla
        isDropped = false;
        RpcResetOdun(baslangicPozisyonu,baslangicRotasyonu); // Tüm istemcilere pozisyonu güncelle
    }

    [ClientRpc]
    private void RpcResetOdun(Vector3 resetPosition,Vector3 baslangicRotasyonu)
    {
        transform.localPosition = resetPosition; // İstemcideki odunun pozisyonunu güncelle
        transform.localEulerAngles = baslangicRotasyonu; // İstemcideki odunun rotasyonunu güncelle
        rb.isKinematic = true; // Odunun fiziksel hareketlerini tekrar etkinleştir
    }
    

    #endregion

    
}