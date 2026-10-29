import type { Alumno, AlumnoCreate, AlumnoUpdate } from '../types/alumno'
import type {
  Materia,
  MateriaCreate,
  MateriaDetalle,
  MateriaUpdate,
} from '../types/materia'

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000'

async function request<T>(
  path: string,
  options: RequestInit = {}
): Promise<T> {
  const url = `${BASE_URL}${path}`
  const response = await fetch(url, {
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
    ...options,
  })

  if (!response.ok) {
    let message = `Error ${response.status}: ${response.statusText}`
    try {
      const body = await response.json()
      if (typeof body.title === 'string') {
        message = body.title
      }
      if (typeof body.detail === 'string') {
        message = `${message}. ${body.detail}`
      }
      if (body.errors && typeof body.errors === 'object') {
        const details = Object.values(body.errors).flat().join('. ')
        if (details) {
          message = `${message}: ${details}`
        }
      }
    } catch {
      // ignore
    }
    throw new Error(message)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json() as Promise<T>
}

export async function listarAlumnos(params?: {
  activo?: boolean
  buscar?: string
}): Promise<Alumno[]> {
  const query = new URLSearchParams()
  if (params?.activo !== undefined) query.set('activo', String(params.activo))
  if (params?.buscar) query.set('buscar', params.buscar)
  const qs = query.toString()
  return request<Alumno[]>(`/api/alumnos${qs ? `?${qs}` : ''}`)
}

export async function obtenerAlumno(id: string): Promise<Alumno> {
  return request<Alumno>(`/api/alumnos/${id}`)
}

export async function crearAlumno(dto: AlumnoCreate): Promise<Alumno> {
  return request<Alumno>('/api/alumnos', {
    method: 'POST',
    body: JSON.stringify(dto),
  })
}

export async function actualizarAlumno(
  id: string,
  dto: AlumnoUpdate
): Promise<Alumno> {
  return request<Alumno>(`/api/alumnos/${id}`, {
    method: 'PUT',
    body: JSON.stringify(dto),
  })
}

export async function cambiarEstadoAlumno(
  id: string,
  activo: boolean
): Promise<Alumno> {
  return request<Alumno>(`/api/alumnos/${id}/estado`, {
    method: 'PATCH',
    body: JSON.stringify({ activo }),
  })
}

export async function listarMaterias(params?: {
  activa?: boolean
  buscar?: string
}): Promise<Materia[]> {
  const query = new URLSearchParams()
  if (params?.activa !== undefined) query.set('activa', String(params.activa))
  if (params?.buscar) query.set('buscar', params.buscar)
  const qs = query.toString()
  return request<Materia[]>(`/api/materias${qs ? `?${qs}` : ''}`)
}

export async function obtenerMateria(id: string): Promise<MateriaDetalle> {
  return request<MateriaDetalle>(`/api/materias/${id}`)
}

export async function crearMateria(dto: MateriaCreate): Promise<Materia> {
  return request<Materia>('/api/materias', {
    method: 'POST',
    body: JSON.stringify(dto),
  })
}

export async function actualizarMateria(
  id: string,
  dto: MateriaUpdate
): Promise<Materia> {
  return request<Materia>(`/api/materias/${id}`, {
    method: 'PUT',
    body: JSON.stringify(dto),
  })
}

export async function cambiarEstadoMateria(
  id: string,
  activa: boolean
): Promise<Materia> {
  return request<Materia>(`/api/materias/${id}/estado`, {
    method: 'PATCH',
    body: JSON.stringify({ activa }),
  })
}

export async function listarMateriasDeAlumno(
  idAlumno: string
): Promise<Materia[]> {
  return request<Materia[]>(`/api/alumnos/${idAlumno}/materias`)
}

export async function asociarMateria(
  idMateria: string,
  idAlumno: string
): Promise<void> {
  await request<void>(`/api/materias/${idMateria}/alumnos/${idAlumno}`, {
    method: 'POST',
  })
}

export async function desasociarMateria(
  idMateria: string,
  idAlumno: string
): Promise<void> {
  await request<void>(`/api/materias/${idMateria}/alumnos/${idAlumno}`, {
    method: 'DELETE',
  })
}
