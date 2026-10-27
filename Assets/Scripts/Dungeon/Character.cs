using UnityEngine;

public class Character : MonoBehaviour
{
    public int maxHealth;
    public int health{get; private set;}
    void Start()
    {
        health=maxHealth;
    }
    public void Hurt(int amount, DamageType type)
    {
        if (type!=DamageType.Unblockable && !PreHurt(amount,type))
        return;
        health=Mathf.Max(0,health-amount);
        AfterHurt(amount,type);
        HealthChanged();
    }
    public virtual bool PreHurt(int amount, DamageType type)
    {
        return true;
    }
    public virtual void AfterHurt(int amount, DamageType type)
    {
    }
    public virtual void Heal(int amount)
    {
        health=Mathf.Clamp(amount,0,maxHealth);
        HealthChanged();
    }
     public virtual void HealthChanged()
    {
    }

}
public enum DamageType
{
    Normal,
    Ice,
    Fire,
    Explosion,
    Unblockable
}