using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PasarNivel : MonoBehaviour

{
    public bool pasarNivel;
    public int IndiceNivel;
    // Start is called before the first frame update
    public void CambiarNivel(int indice)
    {
        SceneManager.LoadScene(indice);
    }

}
