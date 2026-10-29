export interface Materia {
  id: string
  idProfesora: string
  nombre: string
  descripcion: string | null
  activa: boolean
  cantidadAlumnos: number
}

export interface AlumnoResumen {
  id: string
  nombre: string
  apellido: string
  activo: boolean
}

export interface MateriaDetalle {
  id: string
  idProfesora: string
  nombre: string
  descripcion: string | null
  activa: boolean
  alumnos: AlumnoResumen[]
}

export interface MateriaCreate {
  nombre: string
  descripcion?: string
}

export type MateriaUpdate = MateriaCreate
