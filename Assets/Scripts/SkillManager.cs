using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public GameObject LockOnObject;
    public GameObject LaceratedObject;
    static private SkillManager instance = null; // 싱글톤
    public static SkillManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    //
    //
    //
    //
    //
    //
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    IEnumerator DestroyParticleSystem(ParticleSystem part, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (part != null)
        {
            part.Stop();
            Destroy(part.gameObject);
        }
    }

    //
    //
    //
    //
    //
    //
    [SerializeField] bool WaitSkill1 = false;
    public IEnumerator SwordBasicEffect(ParticleSystem part, float scale, Transform weapon, float animSpeed)
    {
        yield return new WaitForSeconds(0.4f / animSpeed);
        weapon.transform.GetChild(0).gameObject.GetComponent<BoxCollider>().enabled = true;
        SoundManager.Instance.CallAudio("SwordA");
        yield return new WaitForSeconds(0.1f / animSpeed);
        ParticleSystem temp = Instantiate(part, PlayerStatus.Instance.transform);
        temp.transform.localScale *= scale;
        temp.transform.SetParent(PlayerStatus.Instance.transform);
        temp.transform.position = gameObject.transform.position + PlayerStatus.Instance.transform.forward;
        Destroy(temp.gameObject, 5f);
        yield return new WaitForSeconds(0.3f / animSpeed);
        weapon.transform.GetChild(0).gameObject.GetComponent<BoxCollider>().enabled = false;
    }
    public IEnumerator SwordSkill1Effect1(ParticleSystem part, float scale, Transform weapon, float animSpeed)
    {
        WaitSkill1 = false;
        weapon.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        ParticleSystem temp = Instantiate(part, weapon);
        SoundManager.Instance.CallAudio("SwordQ1");
        temp.transform.localScale *= scale / 2f;
        Destroy(temp.gameObject, 5f);
        yield return new WaitForSeconds(0.14f / animSpeed);
    }
    public IEnumerator SwordSkill1Effect2(ParticleSystem part, float scale, Transform weapon, float animSpeed)
    {
        yield return new WaitForSeconds(0.4f / animSpeed);
        ParticleSystem temp = Instantiate(part, weapon);
        temp.transform.localScale *= scale / 2f;
        Destroy(temp.gameObject, 5f);
        yield return new WaitForSeconds(0.4f / animSpeed);
        WaitSkill1 = true;
    }
    public IEnumerator SwordSkill1Effect3(ParticleSystem part, float scale, Transform weapon, float animSpeed)
    {
        SoundManager.Instance.CallAudio("SwordQ2");
        yield return new WaitForSeconds(0.1f / animSpeed);
        ParticleSystem temp = Instantiate(part, weapon);
        temp.transform.localScale *= scale / 2f;
        Destroy(temp.gameObject, 5f);
        yield return new WaitForSeconds(0.2f / animSpeed);
        weapon.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        WaitSkill1 = true;
    }
    public IEnumerator SwordSkill2Effect(ParticleSystem part, float scale, WeaponData weapon, float[] Abillity, float animSpeed)
    {
        GameObject temp = Instantiate(weapon.skill2.SkillObject);
        temp.transform.localScale = new Vector3(1, 1, 1) * scale;
        temp.transform.SetLocalPositionAndRotation(PlayerStatus.Instance.transform.position, PlayerStatus.Instance.transform.rotation);
        temp.GetComponent<SwordSkillObject>().InitializeParam = Abillity;

        
        SoundManager.Instance.CallAudio("SwordE");
        for (int i = 0; i < 3; i++)
        {
            ParticleSystem temp2 = Instantiate(part, temp.transform);
            if (i == 2) temp2.transform.Rotate(0, 0, 90);
            yield return new WaitForSeconds(0.2f / animSpeed);
            DestroyParticleSystem(temp2, 1f);
        }
        yield return new WaitForSeconds(0.2f / animSpeed);
    }
    public IEnumerator SwordFlipBurstEffect(ParticleSystem part, float scale, WeaponData weapon, float[] Abillity, float animSpeed)
    {
        GameObject temp = Instantiate(weapon.FlipBurst.SkillObject);
        temp.transform.localScale = new Vector3(1, 1, 1) * scale;
        temp.transform.SetLocalPositionAndRotation(PlayerStatus.Instance.transform.position, PlayerStatus.Instance.transform.rotation);
        temp.GetComponent<SwordSkillObject>().InitializeParam = Abillity;

        ParticleSystem temp2 = Instantiate(part);
        temp2.transform.localScale *= scale;
        temp2.transform.SetLocalPositionAndRotation(PlayerStatus.Instance.transform.position, PlayerStatus.Instance.transform.rotation);
        Destroy(temp2.gameObject, 5f);
        yield return new WaitForSeconds(1f);
        DestroyImmediate(temp);
    }
    public IEnumerator SwordUltimateEffect(ParticleSystem part, float scale, WeaponData weapon, float[] Abillity, float animSpeed)
    {
        yield return new WaitForSeconds(0.4f / animSpeed);
        GameObject temp = Instantiate(weapon.Ultimate.SkillObject);
        temp.transform.localScale = new Vector3(1, 1, 1) * scale;
        temp.transform.SetLocalPositionAndRotation(PlayerStatus.Instance.transform.position, PlayerStatus.Instance.transform.rotation);
        temp.GetComponent<SwordSkillObject>().InitializeParam = Abillity;

        ParticleSystem temp2 = Instantiate(part);
        temp2.transform.localScale *= scale;
        temp2.transform.SetLocalPositionAndRotation(PlayerStatus.Instance.transform.position, PlayerStatus.Instance.transform.rotation);
        DestroyParticleSystem(temp2, 5f);

        yield return new WaitForSeconds(0.75f);
        DestroyImmediate(temp);
    }
    public IEnumerator ApplyDamageOfSkill1(GameObject target, float[] Abillity)
    {
        Debug.Log("Wait " + target.name);
        yield return new WaitUntil(() => WaitSkill1);
        target.gameObject.GetComponent<MonsterStatus>().OnDamaged(Abillity[0], DamageType.Slashing);
        if (target.gameObject.GetComponent<MonsterStatus>().GetDebuff(
        Debuff.Lacerated, DamageType.Slashing, (int)Abillity[2], Abillity[1] * GameManager.Instance.Debuffs[(int)Debuff.Lacerated].AddiDamageRatio))
        {
            PlayerStatus.Instance.HitPoint += Abillity[1] * GameManager.Instance.Debuffs[(int)Debuff.Lacerated].AddiDamageRatio * Abillity[3];
        }
        WaitSkill1 = false;
    }
    //
    //
    // Monster's Debuff
    //
    //
    //
    public bool RangeAddiDamage(Transform Me, ref float dmg)
    {
        if (PlayerStatus.Instance.AddiStats[(int)Stats.RangeMultiplier] > 1)
        {
            float t = Vector3.Distance(Me.position, PlayerStatus.Instance.transform.position) / 15f;
            dmg *= Mathf.Lerp(1f, 1 + PlayerStatus.Instance.AddiStats[(int)Stats.RangeMultiplier], t);
            return true;
        }
        return false;
    }
    
    public bool Bleeding(GameObject target, ref float dmg, float Defense, ref Dictionary<Debuff, int> debuffs, ref float HP)
    {
        bool ReturnValue = false;
        while (debuffs[Debuff.Lacerated] > 1)
        {
            debuffs[Debuff.Lacerated] -= 2;
            HP -= dmg * 100f / (100f + Defense);
            Debug.Log("출혈피해" + dmg * 100f / (100f + Defense));
            ReturnValue = true;
        }
        Debug.Log(debuffs[Debuff.Lacerated]);
        Lacerated(target, ref debuffs);
        return ReturnValue;
    }
    bool Lacerated(GameObject target, ref Dictionary<Debuff, int> debuffs)
    {
        if (target == null) { return false; }
        if (debuffs[Debuff.Lacerated] > 0)
        {
            if (target.transform.childCount == 2)
                Instantiate(LaceratedObject, target.transform).transform.position += Vector3.up * 0;
            else
                target.transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            if (target.transform.childCount == 3)
            {
                target.transform.GetChild(2).gameObject.SetActive(false);

            }
        }
        return true;
    }
    public bool LockOn(GameObject target)
    {
        if (target == null) { return false; }
        Instantiate(LockOnObject, target.transform).transform.position += Vector3.up * 2;
        target.GetComponent<MonsterStatus>().CurDebuffs[Debuff.Exposed] = 1;
        return true;
    }
    public bool FocusAddiDamage(ref Dictionary<Debuff, int> debuffs, ref float dmg)
    {
        if (debuffs[Debuff.Exposed] == 1)
        {
            dmg *= 1 + GameManager.Instance.Debuffs[(int)Debuff.Exposed].AddiDamageRatio;
            return true;
        }
        return false;
    }
    //
    //
    // Player's buff
    //
    //
    //
    public bool BuffAfterBleeding(float DrainValue, bool LvMax)
    {
        if (LvMax)
        {
            if(PlayerStatus.Instance.BuffCor!=null){
                StopCoroutine(PlayerStatus.Instance.BuffCor);
            }
            PlayerStatus.Instance.BuffCor = StartCoroutine(PlayerStatus.Instance.BuffSet(1, 1.5f, 15f));
        }
        PlayerStatus.Instance.HitPoint += DrainValue;
        return true;
    }
}
