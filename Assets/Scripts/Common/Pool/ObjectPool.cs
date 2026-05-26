using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T>
    where T : Component
{
    private readonly T prefab; // 생성할 프리팹
    private readonly Transform parent; // 부모 (생성한 주체)
    private readonly Queue<T> inActive = new Queue<T>(); // 생성된 객체 저장

    public ObjectPool(T prefab, Transform parent, int preLoadCount)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < preLoadCount; i++)
        {
            var obj = Object.Instantiate(this.prefab, this.parent);
            obj.gameObject.SetActive(false);
            inActive.Enqueue(obj); // 미리 생성된 객체는 모두 inActive에 저장
        }
    }

    public T Get()
    {
        T obj;
        if (inActive.Count > 0)
        {
            obj = inActive.Dequeue(); // 스폰될 때 Queue에서 제거됨
        }
        else
        {
            obj = Object.Instantiate(prefab, parent);
        }
        obj.gameObject.SetActive(true); // OnEnable 함수에서 초기화
        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        inActive.Enqueue(obj); // 디스폰되면 다시 Queue로 돌아옴
    }
}