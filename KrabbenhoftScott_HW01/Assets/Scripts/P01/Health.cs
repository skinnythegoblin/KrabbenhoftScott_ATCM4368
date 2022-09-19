using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable, IHealable
{
    [Header("Health Settings")]
    [SerializeField] protected int _maxHealth = 30;
    [SerializeField] protected float _defenseModifier = 0f;
    [SerializeField] protected float _healModifier = 1f;
    [SerializeField] protected float _iFrames = 0.5f;

    [Header("Health Feedback")]
    [SerializeField] protected ParticleSystem _healParticles;
    [SerializeField] protected AudioClip _healSFX;
    [SerializeField] protected ParticleSystem _damageParticles;
    [SerializeField] protected AudioClip _damageSFX;
    [SerializeField] protected ParticleSystem _dieParticles;
    [SerializeField] protected AudioClip _dieSFX;
    [SerializeField] protected Color _damageEmissionColor = Color.black;
    [SerializeField] protected float _SFXVolume = 1f;

    MeshRenderer _meshRenderer;
    Color _initialEmissionColor;
    protected float _lastDamage;
    protected int _currentHealth;

    public virtual void Awake()
    {
        _currentHealth = _maxHealth;
        _lastDamage = _iFrames;
        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer != null)
        {
            _initialEmissionColor = _meshRenderer.material.GetColor("_EmissionColor");
        }
    }

    void FixedUpdate()
    {
        if (_lastDamage < _iFrames)
        {
            _lastDamage += Time.deltaTime;
        }
    }
    
    public virtual void DecreaseHealth(Transform source, int damage)
    {
        if (_lastDamage >= _iFrames)
        {
            _currentHealth = (int)Mathf.Clamp(_currentHealth - (damage * (1f - _defenseModifier)), 0, _maxHealth);
            if (_currentHealth <= 0)
            {
                Kill();
            }
            else
            {
                if (_damageParticles != null)
                {
                    Instantiate(_damageParticles, source.position, source.rotation);
                }
                else if (_meshRenderer != null)
                {
                    StartCoroutine(FlashRed(_iFrames - 0.1f));
                }

                if (_damageSFX != null)
                {
                    AudioHelper.PlayClip3D(_damageSFX, _SFXVolume, transform.position);
                }
            }
            
            _lastDamage = 0;
        }
    }

    public virtual void DecreaseHealth(int damage)
    {
        if (_lastDamage >= _iFrames)
        {
            _currentHealth = (int)Mathf.Clamp(_currentHealth - (damage * (1f - _defenseModifier)), 0, _maxHealth);
            if (_currentHealth <= 0)
            {
                Kill();
            }
            else
            {
                if (_damageParticles != null)
                {
                    Instantiate(_damageParticles, transform);
                }
                else if (_meshRenderer != null)
                {
                    StartCoroutine(FlashRed(_iFrames - 0.1f));
                }

                if (_damageSFX != null)
                {
                    AudioHelper.PlayClip3D(_damageSFX, _SFXVolume, transform.position);
                }
           }
            _lastDamage = 0;
        }
    }

    public void IncreaseHealth(int heal)
    {
        _currentHealth = (int)Mathf.Clamp(_currentHealth + (heal * _healModifier), 0, _maxHealth);
        if (_healParticles != null)
        {
            Instantiate(_healParticles, transform.position, transform.rotation);
        }
        if (_healSFX != null)
        {
            AudioHelper.PlayClip3D(_healSFX, _SFXVolume, transform.position);
        }
    }

    protected virtual void Kill()
    {
        if (_dieParticles != null)
        {
            Instantiate(_dieParticles, transform.position, transform.rotation);
        }
        if (_dieSFX != null)
        {
            AudioHelper.PlayClip3D(_dieSFX, _SFXVolume, transform.position);
        }
        
        gameObject.SetActive(false);
    }

    IEnumerator FlashRed(float duration)
    {
        _meshRenderer.material.SetColor("_EmissionColor", _damageEmissionColor);

        yield return new WaitForSeconds(duration);

        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", _initialEmissionColor);
    }
}
