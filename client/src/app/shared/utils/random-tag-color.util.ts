const TAG_COLORS = [
  '#D7263D', // deep vivid red
  '#A020F0', // vivid purple
  '#3F51B5', // deep indigo
  '#1976D2', // deep blue
  '#0288D1', // deep cyan
  '#009688', // deep teal
  '#388E3C', // deep green
  '#689F38', // olive green
  '#FFA000', // deep amber
  '#F57C00', // deep orange
  '#E65100', // deep burnt orange
  '#C2185B', // deep magenta
  '#7B1FA2', // deep violet
  '#512DA8', // deep purple
  '#303F9F', // deep blue
  '#455A64', // blue grey
  '#607D8B', // slate
  '#00897B', // teal
  '#00695C', // dark teal
  '#5D4037', // deep brown
  '#C62828', // deep red
  '#AD1457', // deep pink
  '#283593', // deep indigo
  '#1565C0', // deep blue
  '#0277BD', // deep cyan
  '#00838F', // deep teal
  '#2E7D32', // deep green
  '#F9A825', // deep yellow
  '#FF6F00', // deep orange
  '#D84315', // deep burnt orange
  '#4E342E', // deep brown
];

export function getRandomTagColor(name: string): string {
  let hash = 0;
  for (let i = 0; i < name.length; i++) {
    hash = name.charCodeAt(i) + ((hash << 5) - hash);
  }
  const index = Math.abs(hash) % TAG_COLORS.length;
  return TAG_COLORS[index];
}