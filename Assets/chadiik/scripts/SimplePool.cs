using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePool<T> where T : class {

	private Queue<T> m_Queue;

	public SimplePool (){

		m_Queue = new Queue<T> ();

	}

	public void Add ( T item ) {

		m_Queue.Enqueue ( item );

	}

	public T Get () {

		if ( m_Queue.Count == 0 ) 
			return null;

		return m_Queue.Dequeue ();

	}

	public void Return ( T item ) {

		m_Queue.Enqueue ( item );

	}

}
