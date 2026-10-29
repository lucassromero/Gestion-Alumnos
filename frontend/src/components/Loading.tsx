export function Loading({ texto = 'Cargando...' }: { texto?: string }) {
  return (
    <div className="flex flex-col items-center justify-center py-14 text-tinta-suave">
      <div className="h-6 w-6 animate-spin rounded-full border-2 border-borde border-t-acento" />
      <p className="mt-4 text-sm">{texto}</p>
    </div>
  )
}
