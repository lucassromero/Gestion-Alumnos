export interface Alumno {
  id: string
  idProfesora: string
  nombre: string
  apellido: string
  email: string | null
  telefono: string | null
  nivelEducativo: string | null
  observaciones: string | null
  activo: boolean
  fechaAlta: string
}

export interface AlumnoCreate {
  nombre: string
  apellido: string
  email?: string
  telefono?: string
  nivelEducativo?: string
  observaciones?: string
}

export type AlumnoUpdate = AlumnoCreate
