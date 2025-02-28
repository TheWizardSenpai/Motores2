using UnityEngine;
using System.Collections.Generic;

public abstract class Skill : MonoBehaviour
{
    [Header("Base Skill")]
    //Nombre de la habilidad
    public string skillName;
    //Duracion de la animacion
    public float animationDuration;
    //Si es autoinfligida o no
    public bool selfInflicted;
    //PreFab para la animacion
    public GameObject effectPrfb;
    //Quien la recibe y quien emite la habilidad
    protected Fighter emitter;
    protected Fighter receiver;
    protected Queue<string> messages;

    void Awake()
    {
        this.messages = new Queue<string>();

    }

    private void Animate()
    {
        if (this.effectPrfb)
            InstantatioEffect();
        //else
        //    Debug.Log("NO  EFECTO!");

    }

    private void InstantatioEffect()
    {
        //Debug.Log("EFECTO!");
        var go = Instantiate(this.effectPrfb, this.receiver.DamagePivot.position, Quaternion.identity);
        Destroy(go, this.animationDuration);
    }

    public void Run()
    {
        if (this.selfInflicted)
        {
            this.receiver = this.emitter;
        }

        this.Animate();

        this.OnRun();
    }

    public void SetEmitterAndReceiver(Fighter _emitter, Fighter _receiver)
    {
        this.emitter = _emitter;
        this.receiver = _receiver;
    }
    public string GetNextMessage()
    {
        if (this.messages.Count != 0)
            return this.messages.Dequeue();
        else
            return null;
    }

    protected abstract void OnRun();
}